#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import argparse
import csv
import json
import os
import re
import sys
import time
from dataclasses import dataclass, asdict
from datetime import datetime, date, timedelta
from typing import Dict, Iterable, List, Optional, Set, Tuple

import requests
from bs4 import BeautifulSoup
from dateutil.relativedelta import relativedelta
from tqdm import tqdm


BASE_BOARD_URL = "https://www.pttweb.cc/bbs/facelift"
SEARCH_TITLE_URL_TEMPLATE = "https://www.pttweb.cc/bbs/facelift/search/t/{keyword}"
USER_AGENT = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121 Safari/537.36"


EYE_BAG_KEYWORDS = [
    "眼袋",
    "下眼瞼",
    "淚溝",
    "黑眼圈",
    "提眼瞼",
    "內開",
    "外開",
    "無刀眼袋",
    "集效電波",
    "補脂",
    "移位",
    "轉位",
]

RECOMMEND_KEYWORDS = [
    "推薦",
    "找",
    "去",
    "做",
    "醫師",
    "醫生",
    "院長",
    "診所",
    "醫院",
    "醫美",
]

# More precise entity patterns:
# - Doctor: 2~6 chars (Chinese/alnum) + suffix (醫師/醫生/院長)
# - Clinic: 2~20 chars (Chinese/alnum) + suffix (診所/醫院/診療所/醫美)
DOCTOR_PAT = re.compile(r"([\u4e00-\u9fffA-Za-z0-9]{2,6})(醫師|醫生|院長)")
CLINIC_PAT = re.compile(r"([\u4e00-\u9fffA-Za-z0-9]{2,20})(診所|醫院|診療所|醫美)")
THREAD_DATE_PAT = re.compile(r"\((\d{4}/\d{2}/\d{2})\s+(\d{2}:\d{2})\)")


CITY_PAT = re.compile(
    r"(台北|新北|桃園|新竹|苗栗|台中|彰化|南投|嘉義|台南|高雄|屏東|宜蘭|花蓮|台東|基隆|金門|馬祖)"
)


def _ensure_dir(path: str) -> None:
    os.makedirs(path, exist_ok=True)


def _http_session(timeout_s: int) -> requests.Session:
    s = requests.Session()
    s.headers.update({"User-Agent": USER_AGENT})
    s.timeout = timeout_s
    return s


def _request_text(session: requests.Session, url: str, timeout_s: int) -> str:
    resp = session.get(url, timeout=timeout_s)
    resp.raise_for_status()
    resp.encoding = resp.apparent_encoding
    return resp.text


def _parse_thread_datetime(text: str) -> Optional[datetime]:
    m = THREAD_DATE_PAT.search(text)
    if not m:
        return None
    d_part = m.group(1)  # YYYY/MM/DD
    t_part = m.group(2)  # HH:MM
    return datetime.strptime(f"{d_part} {t_part}", "%Y/%m/%d %H:%M")


def _normalize_space(s: str) -> str:
    return re.sub(r"\s+", " ", s).strip()


def _extract_thread_title(soup: BeautifulSoup) -> Optional[str]:
    # Most pages have an H1-like header.
    h1 = soup.find(["h1", "h2"])
    if h1 and h1.get_text(strip=True):
        return _normalize_space(h1.get_text())
    # Fallback: locate the first "閒聊/問題/討論/分享/心得/請益" token in headings.
    txt = soup.get_text("\n", strip=True)
    for line in txt.splitlines():
        if line.startswith("[") and "]" in line:
            if any(x in line for x in ["閒聊", "問題", "討論", "分享", "心得", "請益"]):
                return _normalize_space(line)
    return None


def _extract_thread_author(soup: BeautifulSoup) -> Optional[str]:
    # In the WebFetch output it appears as: 作者 [id (nickname)]
    text = soup.get_text("\n", strip=True)
    m = re.search(r"作者\s*\[([^\]]+)\]", text)
    if m:
        return _normalize_space(m.group(1))
    return None


def _extract_thread_text_blocks(soup: BeautifulSoup) -> List[str]:
    # Preserve rough paragraph structure by splitting on blank lines.
    raw = soup.get_text("\n")
    raw = re.sub(r"\r\n", "\n", raw)
    blocks = [b.strip() for b in re.split(r"\n\s*\n+", raw) if b.strip()]
    # Avoid blocks that are purely metadata noise.
    filtered: List[str] = []
    for b in blocks:
        # Skip very short blocks.
        if len(b) < 15:
            continue
        filtered.append(b)
    return filtered


def _extract_entities_from_snippet(snippet: str) -> Tuple[List[str], List[str]]:
    doctors: List[str] = []
    clinics: List[str] = []

    for m in DOCTOR_PAT.finditer(snippet):
        ent = _normalize_space(m.group(1) + m.group(2))
        if ent and ent not in doctors:
            doctors.append(ent)

    # Clinics often end with 診所/醫院/醫美
    for m in CLINIC_PAT.finditer(snippet):
        ent = _normalize_space(m.group(1) + m.group(2))
        if ent and ent not in clinics:
            clinics.append(ent)

    # Fallback: if snippet contains 診所/醫院 but regex missed.
    if ("診所" in snippet or "醫院" in snippet) and not clinics:
        # Grab nearest up-to-20 chars window containing 診所/醫院.
        for tok in ["診所", "醫院"]:
            if tok in snippet:
                # Find all occurrences and extract local windows.
                for idx in [m.start() for m in re.finditer(tok, snippet)]:
                    left = max(0, idx - 12)
                    right = min(len(snippet), idx + len(tok) + 8)
                    cand = _normalize_space(snippet[left:right])
                    if cand and cand not in clinics:
                        clinics.append(cand)
    return doctors, clinics


def _extract_city(snippet: str) -> Optional[str]:
    m = CITY_PAT.search(snippet)
    if m:
        return m.group(1)
    return None


def _contains_any(s: str, keywords: List[str]) -> bool:
    return any(k in s for k in keywords)


def _is_eye_bag_related_block(block: str) -> bool:
    return _contains_any(block, EYE_BAG_KEYWORDS)


def _is_recommend_related_block(block: str) -> bool:
    # Require at least "recommend-ish" token AND doctor/clinic token.
    recommend_like = any(k in block for k in ["推薦", "找", "去"])
    doctor_or_clinic = any(k in block for k in ["醫師", "醫生", "院長", "診所", "醫院", "醫美"])
    return (recommend_like and doctor_or_clinic) or doctor_or_clinic


def parse_thread_page(
    session: requests.Session,
    thread_url: str,
    timeout_s: int,
    require_evidence: bool = True,
) -> Optional[Dict]:
    html = _request_text(session, thread_url, timeout_s=timeout_s)
    soup = BeautifulSoup(html, "lxml")

    thread_text = soup.get_text("\n")
    dt = _parse_thread_datetime(thread_text)
    if dt is None:
        # Some pages might not include the exact timestamp. Still try to proceed.
        dt = datetime.min

    title = _extract_thread_title(soup)
    author = _extract_thread_author(soup)
    blocks = _extract_thread_text_blocks(soup)

    eye_evidences: List[Dict] = []
    for b in blocks:
        if not _is_eye_bag_related_block(b):
            continue
        if not _is_recommend_related_block(b):
            continue

        doctors, clinics = _extract_entities_from_snippet(b)
        if not doctors and not clinics:
            # Keep a small snippet as "evidence" if it looks like a suggestion.
            # This still helps the user validate manually.
            b_trim = _normalize_space(b)
            if len(b_trim) > 0:
                doctors, clinics = [], []
        city = _extract_city(b)

        snippet = _normalize_space(b)
        if len(snippet) > 260:
            snippet = snippet[:260] + "..."

        eye_evidences.append(
            {
                "snippet": snippet,
                "doctors": doctors,
                "clinics": clinics,
                "city": city,
            }
        )

    if not eye_evidences and require_evidence:
        return None

    return {
        "thread_url": thread_url,
        "title": title,
        "author": author,
        "datetime": dt.isoformat() if dt != datetime.min else None,
        "eye_bag_evidences": eye_evidences,
    }


def _parse_search_results(html: str) -> Tuple[Set[str], Optional[str]]:
    soup = BeautifulSoup(html, "lxml")
    thread_urls: Set[str] = set()

    # Thread links look like /bbs/facelift/M.1773903060.A.A91
    for a in soup.find_all("a", href=True):
        href = a["href"]
        if href.startswith("/bbs/facelift/M."):
            thread_urls.add("https://www.pttweb.cc" + href)

    next_url = None
    # Follow "下一頁"
    for a in soup.find_all("a", href=True):
        txt = a.get_text(strip=True)
        if "下一頁" in txt or "下一頁" in a.get("aria-label", ""):
            href = a["href"]
            if href.startswith("http"):
                next_url = href
            elif href.startswith("/"):
                next_url = "https://www.pttweb.cc" + href
            else:
                next_url = "https://www.pttweb.cc" + "/" + href
            break

    if next_url is None:
        # Fallback: look for rel="next"
        rel_next = soup.find("a", attrs={"rel": "next"})
        if rel_next and rel_next.get("href"):
            href = rel_next["href"]
            if href.startswith("http"):
                next_url = href
            elif href.startswith("/"):
                next_url = "https://www.pttweb.cc" + href

    return thread_urls, next_url


def crawl_threads_by_title_keywords(
    session: requests.Session,
    keywords: List[str],
    timeout_s: int,
    max_pages_per_keyword: int,
    polite_delay_s: float,
) -> Set[str]:
    all_threads: Set[str] = set()

    for keyword in keywords:
        start_url = SEARCH_TITLE_URL_TEMPLATE.format(keyword=requests.utils.quote(keyword))
        page_url = start_url
        page_count = 0

        while page_url and page_count < max_pages_per_keyword:
            page_count += 1
            html = _request_text(session, page_url, timeout_s=timeout_s)
            thread_urls, next_url = _parse_search_results(html)
            before = len(all_threads)
            all_threads.update(thread_urls)
            after = len(all_threads)

            print(
                f"[search] keyword={keyword} page={page_count} new_threads={after-before} total_threads={after}",
                file=sys.stderr,
            )

            time.sleep(polite_delay_s)
            page_url = next_url

    return all_threads


def _parse_board_thread_entries(html: str) -> List[Tuple[str, str]]:
    """
    從看板分頁抓每篇文章的 (title, thread_url)。
    """
    soup = BeautifulSoup(html, "lxml")
    entries: List[Tuple[str, str]] = []
    for a in soup.find_all("a", href=True):
        href = a["href"]
        if href.startswith("/bbs/facelift/M.") and "A." in href:
            title = _normalize_space(a.get_text(strip=True))
            if not title:
                continue
            entries.append((title, "https://www.pttweb.cc" + href))

    # De-dup same URL (keep first title)
    seen: Set[str] = set()
    uniq: List[Tuple[str, str]] = []
    for t, u in entries:
        if u in seen:
            continue
        seen.add(u)
        uniq.append((t, u))
    return uniq


def _extract_next_board_url(html: str) -> Optional[str]:
    soup = BeautifulSoup(html, "lxml")
    for a in soup.find_all("a", href=True):
        txt = a.get_text(strip=True)
        if "下一頁" in txt:
            href = a["href"]
            if href.startswith("http"):
                return href
            if href.startswith("/"):
                return "https://www.pttweb.cc" + href
            return "https://www.pttweb.cc/" + href
    return None


def crawl_threads_by_board(
    session: requests.Session,
    start_board_url: str,
    timeout_s: int,
    start_dt: datetime,
    end_dt: datetime,
    title_keywords: List[str],
    max_board_pages: int,
    polite_delay_s: float,
) -> List[Dict]:
    """
    以看板分頁往更舊方向走，遇到標題含眼袋關鍵字的文章才抓文章頁。

    Stop 方式（在標題關鍵字假設成立時比較準確）：
    當某一頁上至少找到一篇「標題含眼袋關鍵字」的文章，但這些文章全部都比 start_dt 還舊，
    則後續分頁只會更舊，因此可停止。
    """
    results: List[Dict] = []
    processed_thread_urls: Set[str] = set()

    board_url = start_board_url
    board_page_count = 0
    stop = False

    while board_url and board_page_count < max_board_pages and not stop:
        board_page_count += 1
        html = _request_text(session, board_url, timeout_s=timeout_s)
        entries = _parse_board_thread_entries(html)

        title_matched_threads: List[Tuple[str, str]] = []
        for title, thread_url in entries:
            if any(k in title for k in title_keywords):
                title_matched_threads.append((title, thread_url))

        print(
            f"[board] page={board_page_count} board_url={board_url} "
            f"entries={len(entries)} matched_titles={len(title_matched_threads)}",
            file=sys.stderr,
        )

        any_matching_in_range = False
        any_matching_found = False

        for _title, thread_url in title_matched_threads:
            if thread_url in processed_thread_urls:
                continue
            processed_thread_urls.add(thread_url)

            any_matching_found = True
            # For stop-condition we need datetime even when evidences are empty.
            data = parse_thread_page(
                session=session,
                thread_url=thread_url,
                timeout_s=timeout_s,
                require_evidence=False,
            )
            if data is None:
                continue

            dt_s = data.get("datetime")
            if not dt_s:
                any_matching_in_range = True
                # If timestamp is missing, keep it only when evidences exist.
                if data.get("eye_bag_evidences"):
                    results.append(data)
                continue

            dt = datetime.fromisoformat(dt_s)
            if start_dt <= dt <= end_dt:
                if data.get("eye_bag_evidences"):
                    results.append(data)
                any_matching_in_range = True

        if any_matching_found and not any_matching_in_range:
            stop = True
            break

        time.sleep(polite_delay_s)
        board_url = _extract_next_board_url(html)

    return results


def parse_args() -> argparse.Namespace:
    ap = argparse.ArgumentParser()
    ap.add_argument("--out-dir", default="scripts/ptt_eye_bag_output")
    ap.add_argument("--days-back", type=int, default=None, help="用天數回溯（優先於 --years-back）")
    ap.add_argument("--years-back", type=int, default=5)
    ap.add_argument("--crawl-mode", default="search", choices=["search", "board"])
    ap.add_argument("--timeout-s", type=int, default=25)
    ap.add_argument("--polite-delay-s", type=float, default=0.9)
    ap.add_argument("--max-pages-per-keyword", type=int, default=60)
    ap.add_argument("--keywords", default="眼袋,淚溝,黑眼圈,下眼瞼,提眼瞼")
    ap.add_argument("--max-threads", type=int, default=0, help="0=不限制（安全起見請不要太大）")
    ap.add_argument("--start-board-url", default="https://www.pttweb.cc/bbs/facelift/page?n=49325")
    ap.add_argument("--max-board-pages", type=int, default=6000)
    ap.add_argument("--board-title-keywords", default="眼袋,淚溝,黑眼圈,下眼瞼,提眼瞼")
    return ap.parse_args()


def main() -> int:
    args = parse_args()
    out_dir = os.path.abspath(args.out_dir)
    _ensure_dir(out_dir)

    if args.days_back is not None:
        start_dt = datetime.now() - timedelta(days=args.days_back)
    else:
        start_dt = datetime.now() - relativedelta(years=args.years_back)
    end_dt = datetime.now()

    keywords = [k.strip() for k in args.keywords.split(",") if k.strip()]

    session = _http_session(timeout_s=args.timeout_s)

    results: List[Dict] = []
    errors: List[Dict] = []
    if args.crawl_mode == "search":
        threads = crawl_threads_by_title_keywords(
            session=session,
            keywords=keywords,
            timeout_s=args.timeout_s,
            max_pages_per_keyword=args.max_pages_per_keyword,
            polite_delay_s=args.polite_delay_s,
        )

        threads_list = sorted(threads)
        if args.max_threads and args.max_threads > 0:
            threads_list = threads_list[: args.max_threads]

        print(f"[main] total candidate threads={len(threads_list)}", file=sys.stderr)

        for thread_url in tqdm(threads_list, desc="threads"):
            try:
                data = parse_thread_page(session=session, thread_url=thread_url, timeout_s=args.timeout_s)
                if data is None:
                    continue

                dt_s = data.get("datetime")
                if dt_s:
                    dt = datetime.fromisoformat(dt_s)
                    # Filter to 5 years window.
                    if not (start_dt <= dt <= end_dt):
                        continue

                results.append(data)
                time.sleep(args.polite_delay_s)
            except Exception as e:
                errors.append({"thread_url": thread_url, "error": str(e)})
                time.sleep(args.polite_delay_s)
    else:
        title_keywords = [k.strip() for k in args.board_title_keywords.split(",") if k.strip()]
        print(f"[main] board mode: start_board_url={args.start_board_url} title_keywords={title_keywords}", file=sys.stderr)
        try:
            results = crawl_threads_by_board(
                session=session,
                start_board_url=args.start_board_url,
                timeout_s=args.timeout_s,
                start_dt=start_dt,
                end_dt=end_dt,
                title_keywords=title_keywords,
                max_board_pages=args.max_board_pages,
                polite_delay_s=args.polite_delay_s,
            )
        except Exception as e:
            errors.append({"mode": "board", "error": str(e)})

    # Save JSON
    json_path = os.path.join(out_dir, "ptt_eye_bag_recommendations.json")
    with open(json_path, "w", encoding="utf-8") as f:
        json.dump(results, f, ensure_ascii=False, indent=2)

    # Save CSV flattened rows
    csv_path = os.path.join(out_dir, "ptt_eye_bag_recommendations.csv")
    with open(csv_path, "w", encoding="utf-8", newline="") as f:
        w = csv.writer(f)
        w.writerow(
            [
                "thread_title",
                "thread_url",
                "thread_author",
                "thread_datetime",
                "snippet",
                "doctors",
                "clinics",
                "city",
            ]
        )
        for r in results:
            for ev in r.get("eye_bag_evidences", []):
                w.writerow(
                    [
                        r.get("title"),
                        r.get("thread_url"),
                        r.get("author"),
                        r.get("datetime"),
                        ev.get("snippet"),
                        ",".join(ev.get("doctors", [])),
                        ",".join(ev.get("clinics", [])),
                        ev.get("city"),
                    ]
                )

    # Save errors
    err_path = os.path.join(out_dir, "errors.json")
    with open(err_path, "w", encoding="utf-8") as f:
        json.dump(errors, f, ensure_ascii=False, indent=2)

    # Aggregate: group by (doctor, clinic)
    agg: Dict[str, Dict] = {}
    for r in results:
        for ev in r.get("eye_bag_evidences", []):
            clinics = ev.get("clinics") or []
            doctors = ev.get("doctors") or []

            if not clinics and not doctors:
                continue

            key = "|".join(sorted(set(doctors + clinics)))
            if key not in agg:
                agg[key] = {
                    "entity": key,
                    "doctors": sorted(set(doctors)),
                    "clinics": sorted(set(clinics)),
                    "cities": sorted(set([ev.get("city")] if ev.get("city") else [])),
                    "evidence_count": 0,
                    "threads": set(),
                }
            agg[key]["evidence_count"] += 1
            agg[key]["threads"].add(r.get("thread_url"))
            if ev.get("city"):
                agg[key]["cities"] = sorted(set(agg[key]["cities"] + [ev.get("city")]))

    summary = sorted(agg.values(), key=lambda x: x["evidence_count"], reverse=True)
    summary_path = os.path.join(out_dir, "summary_top_entities.json")
    for s in summary:
        s["threads"] = sorted(list(s["threads"]))
    with open(summary_path, "w", encoding="utf-8") as f:
        json.dump(summary, f, ensure_ascii=False, indent=2)

    # Print a small summary for console
    print(f"\n[done] matched threads={len(results)}", file=sys.stderr)
    print(f"[done] out_dir={out_dir}", file=sys.stderr)
    print(f"[done] json={json_path}", file=sys.stderr)
    print(f"[done] csv={csv_path}", file=sys.stderr)
    if errors:
        print(f"[done] errors={len(errors)} (see {err_path})", file=sys.stderr)
    else:
        print("[done] errors=0", file=sys.stderr)

    # Also print top 10 entities to stdout
    print("\nTop entities (by evidence count):")
    for ent in summary[:10]:
        print(f"- {ent['entity']} | evidences={ent['evidence_count']} | threads={len(ent['threads'])}")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())

