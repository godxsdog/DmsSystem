# -*- coding: utf-8 -*-
"""修復 NEW_C_FAS.FUND_INSERT.sql 中已附加的 INSERT：FUND_NO 改為 C13..C59，NAME/SNAME 改為 CSV 對應值"""
import csv
import os
import re

BASE = os.path.dirname(os.path.abspath(__file__))
CSV_PATH = os.path.join(BASE, 'CG基金清單all_FAS191 1.csv')
SQL_PATH = os.path.join(BASE, 'NEW_C_FAS.FUND_INSERT.sql')

def col(d, k):
    return d.get(k, '').strip() if isinstance(d, dict) else ''

def esc(s):
    if s is None or s == '': return ''
    return str(s).replace("'", "''")

def main():
    with open(CSV_PATH, 'r', encoding='utf-8') as f:
        rows = list(csv.DictReader(f))

    try:
        with open(SQL_PATH, 'r', encoding='utf-8') as f:
            sql_lines = f.readlines()
        out_enc = 'utf-8'
    except UnicodeDecodeError:
        with open(SQL_PATH, 'r', encoding='cp950') as f:
            sql_lines = f.readlines()
        out_enc = 'cp950'

    # 從第 16 行（0-based index 15）起為先前附加的 INSERT，且開頭為 values ('C9' 者需修正
    start_sql_idx = 15
    start_csv_idx = 12   # 對應 CSV 第 13 筆
    fixed = 0
    out_lines = list(sql_lines[:start_sql_idx])

    for j, line in enumerate(sql_lines[start_sql_idx:]):
        if " values (" not in line:
            out_lines.append(line)
            continue
        parts = line.split(" values (", 1)
        if len(parts) != 2:
            out_lines.append(line)
            continue
        prefix = parts[0] + " values ("
        rest = parts[1]
        if not rest.rstrip().endswith(");"):
            out_lines.append(line)
            continue
        v = rest.rsplit(");", 1)[0]
        if not v.strip().startswith("'C9'"):
            out_lines.append(line)
            continue
        csv_idx = start_csv_idx + j
        if csv_idx >= len(rows):
            out_lines.append(line)
            continue
        r = rows[csv_idx]
        fund_no = f'C{csv_idx + 1}'
        name = esc(col(r, '基金中文名稱'))
        sname = esc(col(r, '基金簡稱'))

        # 1) 將開頭的 'C9' 改為正確 FUND_NO（僅改 values 內第一個）
        v = re.sub(r"^'C9'", f"'{fund_no}'", v, count=1)
        # 2) 將第 2、3 個值（NAME, SNAME）改為 CSV 的 基金中文名稱、基金簡稱
        v = re.sub(r"(,'(?:[^']|'')*','(?:[^']|'')*')", f",N'{name}',N'{sname}'", v, count=1)
        out_lines.append(prefix + v + ");\n")
        fixed += 1

    with open(SQL_PATH, 'w', encoding=out_enc) as f:
        f.writelines(out_lines)
    print(f'Fixed {fixed} INSERT rows (FUND_NO + NAME/SNAME) in {SQL_PATH}')

if __name__ == '__main__':
    main()
