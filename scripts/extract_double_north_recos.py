#!/usr/bin/env python3
# -*- coding: utf-8 -*-

"""
從 `ptt_eye_bag_recommendations.json` 整理雙北（台北/新北）醫師與診所：
- 推薦/不推薦 分開統計
- 附上證據 snippet 與 thread_url

用法：
  scripts/pyenv/bin/python3 scripts/extract_double_north_recos.py \
    --in-json scripts/ptt_eye_bag_output_full_v2/ptt_eye_bag_recommendations.json \
    --out-dir scripts/ptt_eye_bag_output_full_v2/double_north_report
"""

import argparse
import csv
import json
import os
import re
from collections import defaultdict
from dataclasses import dataclass
from typing import Dict, List, Optional, Tuple


DOUBLE_NORTH = {"台北", "新北"}

# sentiment keywords (very simple heuristics)
NEG_KWS = ["不推", "不推薦", "踩雷", "雷", "黑店", "避雷", "不要去", "別去", "拒絕", "很雷"]
POS_KWS = ["推薦", "推", "很專業", "做得很好", "內開做得很好", "效果很好", "值得", "漂亮", "滿意"]

DOCTOR_CANON_PAT = re.compile(r"([\u4e00-\u9fff]{2,4})(醫師|醫生|院長)")
CLINIC_CANON_PAT = re.compile(r"([\u4e00-\u9fffA-Za-z0-9]{2,20})(診所|醫院|診療所|醫美)")

# 去掉常見前綴動詞/介詞
PREFIX_STRIP = [
    "一起給",
    "給",
    "我找",
    "是去找",
    "去找",
    "推薦",
    "推",
    "請推",
    "蠻多人推薦",
]


def normalize_entity(raw: str) -> str:
    s = raw.strip()
    for p in PREFIX_STRIP:
        if s.startswith(p):
            s = s[len(p) :].strip()
    # 嘗試抽出規範的「姓名+醫師/醫生/院長」或「名稱+診所/醫院/醫美」
    m = DOCTOR_CANON_PAT.search(s)
    if m:
        return m.group(1) + m.group(2)
    m = CLINIC_CANON_PAT.search(s)
    if m:
        return m.group(1) + m.group(2)
    return s


def sentiment(snippet: str) -> Optional[str]:
    t = snippet.strip()
    # Negative first (avoid matching "推" inside "不推")
    if any(k in t for k in NEG_KWS):
        return "not_recommended"
    if any(k in t for k in POS_KWS):
        return "recommended"
    return None


def is_double_north(ev_city: Optional[str], snippet: str, title: str) -> bool:
    if ev_city in DOUBLE_NORTH:
        return True
    if any(c in snippet for c in DOUBLE_NORTH):
        return True
    if any(c in title for c in DOUBLE_NORTH):
        return True
    if "雙北" in snippet or "雙北" in title:
        return True
    return False


@dataclass
class Evidence:
    entity_type: str  # doctor|clinic
    entity: str
    sentiment: str  # recommended|not_recommended|unknown
    thread_url: str
    title: str
    datetime: Optional[str]
    city: Optional[str]
    snippet: str


def parse_args() -> argparse.Namespace:
    ap = argparse.ArgumentParser()
    ap.add_argument("--in-json", required=True)
    ap.add_argument("--out-dir", required=True)
    return ap.parse_args()


def main() -> int:
    args = parse_args()
    with open(args.in_json, "r", encoding="utf-8") as f:
        data = json.load(f)

    evidences: List[Evidence] = []

    for thread in data:
        turl = thread.get("thread_url")
        title = thread.get("title") or ""
        dt = thread.get("datetime")
        for ev in thread.get("eye_bag_evidences", []):
            snippet = ev.get("snippet") or ""
            ev_city = ev.get("city")
            if not is_double_north(ev_city, snippet, title):
                continue

            s = sentiment(snippet) or "unknown"

            for d in ev.get("doctors", []) or []:
                ent = normalize_entity(d)
                evidences.append(
                    Evidence(
                        entity_type="doctor",
                        entity=ent,
                        sentiment=s,
                        thread_url=turl,
                        title=title,
                        datetime=dt,
                        city=ev_city,
                        snippet=snippet,
                    )
                )
            for c in ev.get("clinics", []) or []:
                ent = normalize_entity(c)
                evidences.append(
                    Evidence(
                        entity_type="clinic",
                        entity=ent,
                        sentiment=s,
                        thread_url=turl,
                        title=title,
                        datetime=dt,
                        city=ev_city,
                        snippet=snippet,
                    )
                )

    os.makedirs(args.out_dir, exist_ok=True)

    # Aggregate counts
    agg: Dict[Tuple[str, str], Dict] = defaultdict(lambda: {"recommended": 0, "not_recommended": 0, "unknown": 0, "threads": set(), "evidences": []})

    for e in evidences:
        key = (e.entity_type, e.entity)
        agg[key][e.sentiment] += 1
        agg[key]["threads"].add(e.thread_url)
        agg[key]["evidences"].append(e)

    # Write CSV (one row per entity)
    out_csv = os.path.join(args.out_dir, "double_north_entities.csv")
    rows = []
    for (etype, ent), v in agg.items():
        rows.append(
            {
                "entity_type": etype,
                "entity": ent,
                "recommended_count": v["recommended"],
                "not_recommended_count": v["not_recommended"],
                "unknown_count": v["unknown"],
                "thread_count": len(v["threads"]),
                "threads": " | ".join(sorted(v["threads"])),
            }
        )
    rows.sort(key=lambda r: (r["not_recommended_count"], r["recommended_count"], r["thread_count"]), reverse=True)

    with open(out_csv, "w", encoding="utf-8", newline="") as f:
        w = csv.DictWriter(
            f,
            fieldnames=[
                "entity_type",
                "entity",
                "recommended_count",
                "not_recommended_count",
                "unknown_count",
                "thread_count",
                "threads",
            ],
        )
        w.writeheader()
        w.writerows(rows)

    # Write detailed evidences CSV (one row per evidence)
    out_ev_csv = os.path.join(args.out_dir, "double_north_evidences.csv")
    with open(out_ev_csv, "w", encoding="utf-8", newline="") as f:
        w = csv.writer(f)
        w.writerow(["entity_type", "entity", "sentiment", "city", "thread_datetime", "thread_url", "thread_title", "snippet"])
        for e in evidences:
            w.writerow([e.entity_type, e.entity, e.sentiment, e.city, e.datetime, e.thread_url, e.title, e.snippet])

    # Also dump JSON
    out_json = os.path.join(args.out_dir, "double_north_entities.json")
    out_payload = []
    for (etype, ent), v in agg.items():
        out_payload.append(
            {
                "entity_type": etype,
                "entity": ent,
                "recommended_count": v["recommended"],
                "not_recommended_count": v["not_recommended"],
                "unknown_count": v["unknown"],
                "thread_count": len(v["threads"]),
                "threads": sorted(v["threads"]),
            }
        )
    out_payload.sort(key=lambda r: (r["not_recommended_count"], r["recommended_count"], r["thread_count"]), reverse=True)
    with open(out_json, "w", encoding="utf-8") as f:
        json.dump(out_payload, f, ensure_ascii=False, indent=2)

    print(f"[done] out_dir={args.out_dir}")
    print(f"[done] entities_csv={out_csv}")
    print(f"[done] evidences_csv={out_ev_csv}")
    print(f"[done] entities_json={out_json}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

