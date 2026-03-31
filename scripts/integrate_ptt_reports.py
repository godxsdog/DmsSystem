#!/usr/bin/env python3
# -*- coding: utf-8 -*-

from __future__ import annotations

import argparse
import csv
import re
import shutil
from dataclasses import dataclass
from pathlib import Path
from typing import Iterable


DOCTOR_CANON = re.compile(r"^([\u4e00-\u9fffA-Za-z0-9]{2,6})(醫師|醫生|院長)$")
CLINIC_CANON = re.compile(r"^([\u4e00-\u9fffA-Za-z0-9]{2,20})(診所|醫院|診療所|醫美)$")

# 常見「句子片段」或「泛稱」造成的假實體
NOISE_SUBSTRINGS = [
    "請問",
    "想問",
    "有人知道",
    "有沒有",
    "求推薦",
    "推薦",
    "不推薦",
    "不推",
    "雙北",
    "台北",
    "新北",
    "眼袋",
    "淚溝",
    "黑眼圈",
    "手術",
    "醫美",
    "診所",
    "醫院",
]


@dataclass(frozen=True)
class EntityRow:
    entity_type: str
    entity: str
    recommended_count: int
    not_recommended_count: int
    unknown_count: int
    thread_count: int
    threads: str


def parse_args() -> argparse.Namespace:
    ap = argparse.ArgumentParser(description="整合 PTT 眼袋爬蟲與雙北報表，並輸出去噪總表。")
    ap.add_argument("--src-out-dir", required=True, help="爬蟲 out_dir，例如 scripts/ptt_eye_bag_output_board_full")
    ap.add_argument("--final-dir", required=True, help="整合後輸出資料夾（會建立）")
    ap.add_argument("--min_total_mentions", type=int, default=1, help="最少提及次數門檻（推+不推+未知）")
    ap.add_argument("--top-k", type=int, default=50, help="摘要輸出前幾名")
    return ap.parse_args()


def _read_entities_csv(path: Path) -> list[EntityRow]:
    rows: list[EntityRow] = []
    with path.open("r", encoding="utf-8", newline="") as f:
        reader = csv.DictReader(f)
        for r in reader:
            rows.append(
                EntityRow(
                    entity_type=r["entity_type"],
                    entity=r["entity"],
                    recommended_count=int(r["recommended_count"]),
                    not_recommended_count=int(r["not_recommended_count"]),
                    unknown_count=int(r["unknown_count"]),
                    thread_count=int(r["thread_count"]),
                    threads=r.get("threads", ""),
                )
            )
    return rows


def _total_mentions(r: EntityRow) -> int:
    return r.recommended_count + r.not_recommended_count + r.unknown_count


def _looks_noisy(entity: str) -> bool:
    e = entity.strip()
    if not e:
        return True
    # 太長通常是句子
    if len(e) > 14:
        return True
    if any(s in e for s in NOISE_SUBSTRINGS):
        # 但若是「真正的名稱」：例如 台生診所、群英醫院 等，仍會被包含「診所/醫院」
        # 這裡只針對明顯句子片段（含空白/標點）提高過濾力
        if any(ch in e for ch in " ，。！？?/()（）:："):
            return True
        # 若含「推薦/請問/想問/有人知道/有沒有/求推薦」等語氣詞，幾乎必為句子片段
        if any(x in e for x in ["請問", "想問", "有人知道", "有沒有", "求推薦"]):
            return True
    return False


def _is_canonical(entity_type: str, entity: str) -> bool:
    if entity_type == "doctor":
        return bool(DOCTOR_CANON.match(entity))
    if entity_type == "clinic":
        return bool(CLINIC_CANON.match(entity))
    return False


def clean_entities(rows: Iterable[EntityRow], *, min_total_mentions: int) -> tuple[list[EntityRow], list[EntityRow]]:
    doctors: list[EntityRow] = []
    clinics: list[EntityRow] = []

    for r in rows:
        if _total_mentions(r) < min_total_mentions:
            continue
        if _looks_noisy(r.entity):
            continue
        if not _is_canonical(r.entity_type, r.entity):
            continue
        if r.entity_type == "doctor":
            doctors.append(r)
        elif r.entity_type == "clinic":
            clinics.append(r)

    # 主要依「推/不推」排序，讓有爭議的也能浮上來
    doctors.sort(key=lambda x: (x.not_recommended_count, x.recommended_count, x.unknown_count), reverse=True)
    clinics.sort(key=lambda x: (x.not_recommended_count, x.recommended_count, x.unknown_count), reverse=True)
    return doctors, clinics


def write_entities_csv(path: Path, rows: list[EntityRow]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8", newline="") as f:
        w = csv.writer(f)
        w.writerow(
            [
                "entity_type",
                "entity",
                "recommended_count",
                "not_recommended_count",
                "unknown_count",
                "thread_count",
                "threads",
                "total_mentions",
            ]
        )
        for r in rows:
            w.writerow(
                [
                    r.entity_type,
                    r.entity,
                    r.recommended_count,
                    r.not_recommended_count,
                    r.unknown_count,
                    r.thread_count,
                    r.threads,
                    _total_mentions(r),
                ]
            )


def _top_lines(rows: list[EntityRow], k: int) -> str:
    out = []
    for r in rows[:k]:
        out.append(
            f"- {r.entity}：推={r.recommended_count} / 不推={r.not_recommended_count} / 未知={r.unknown_count}（文章數={r.thread_count}）"
        )
    return "\n".join(out) if out else "- （無）"


def write_markdown(path: Path, doctors: list[EntityRow], clinics: list[EntityRow], *, top_k: int) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    content = f"""## 雙北（台北＋新北）眼袋相關：去噪後實體總表

此份名單是從 `double_north_report/double_north_entities.csv` 再做一次去噪（僅保留符合「姓名+職稱 / 診所或醫院名+類型」的實體），避免把「請問/想問/推薦」等句子片段算成醫師/診所名稱。

### 醫師（Top {top_k}）

{_top_lines(doctors, top_k)}

### 診所／醫院（Top {top_k}）

{_top_lines(clinics, top_k)}
"""
    path.write_text(content, encoding="utf-8")


def _copy_if_exists(src: Path, dst: Path) -> None:
    if src.exists():
        dst.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(src, dst)


def main() -> int:
    args = parse_args()
    src_out_dir = Path(args.src_out_dir).resolve()
    final_dir = Path(args.final_dir).resolve()

    # 來源檔案
    crawler_json = src_out_dir / "ptt_eye_bag_recommendations.json"
    crawler_csv = src_out_dir / "ptt_eye_bag_recommendations.csv"
    summary_json = src_out_dir / "summary_top_entities.json"
    errors_json = src_out_dir / "errors.json"
    dn_dir = src_out_dir / "double_north_report"
    dn_entities = dn_dir / "double_north_entities.csv"
    dn_evidences = dn_dir / "double_north_evidences.csv"
    dn_entities_json = dn_dir / "double_north_entities.json"

    if not dn_entities.exists():
        raise SystemExit(f"找不到 {dn_entities}，請先跑 extract_double_north_recos.py")

    # 建立 final 目錄與複製原始檔
    final_dir.mkdir(parents=True, exist_ok=True)
    _copy_if_exists(crawler_json, final_dir / "raw" / crawler_json.name)
    _copy_if_exists(crawler_csv, final_dir / "raw" / crawler_csv.name)
    _copy_if_exists(summary_json, final_dir / "raw" / summary_json.name)
    _copy_if_exists(errors_json, final_dir / "raw" / errors_json.name)
    _copy_if_exists(dn_entities, final_dir / "double_north_report" / dn_entities.name)
    _copy_if_exists(dn_evidences, final_dir / "double_north_report" / dn_evidences.name)
    _copy_if_exists(dn_entities_json, final_dir / "double_north_report" / dn_entities_json.name)

    # 去噪整合
    rows = _read_entities_csv(dn_entities)
    doctors, clinics = clean_entities(rows, min_total_mentions=args.min_total_mentions)

    write_entities_csv(final_dir / "integrated" / "double_north_doctors_clean.csv", doctors)
    write_entities_csv(final_dir / "integrated" / "double_north_clinics_clean.csv", clinics)
    write_markdown(final_dir / "integrated" / "SUMMARY_double_north_clean.md", doctors, clinics, top_k=args.top_k)

    print(f"[done] final_dir={final_dir}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

