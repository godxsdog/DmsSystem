# -*- coding: utf-8 -*-
"""從 CG基金清單all_FAS191 1.csv 第13筆起產生 FAS.FUND INSERT，補足 NEW_C_FAS.FUND_INSERT.sql"""
import csv
import os
import re

BASE = os.path.dirname(os.path.abspath(__file__))
CSV_PATH = os.path.join(BASE, 'CG基金清單all_FAS191 1.csv')
SQL_PATH = os.path.join(BASE, 'NEW_C_FAS.FUND_INSERT.sql')

def col(d, k):
    return d.get(k, '').strip() if isinstance(d, dict) else ''

def parse_date(s):
    if not s: return "to_date('1900-01-01 00:00:00','YYYY-MM-DD HH24:MI:SS')"
    s = s.strip().replace(' ', '')
    m = re.match(r'(\d{4})/(\d{1,2})/(\d{1,2})', s)
    if m:
        y, mo, d = m.group(1), m.group(2).zfill(2), m.group(3).zfill(2)
        return f"to_date('{y}-{mo}-{d} 00:00:00','YYYY-MM-DD HH24:MI:SS')"
    return "to_date('1900-01-01 00:00:00','YYYY-MM-DD HH24:MI:SS')"

def esc(s):
    if s is None or s == '': return ''
    return str(s).replace("'", "''")

def currency_map(s):
    m = {'歐元':'EUR','美元':'USD','澳幣':'AUD'}
    return m.get(str(s).strip(), 'USD' if s and '美元' in str(s) else 'EUR')

def area_map(s):
    a = str(s).strip()
    if '歐洲' in a: return 'EU'
    if '美國' in a: return 'US'
    return 'GB'

def fund_type_map(kind2):
    k = str(kind2).strip()
    if '債券' in k or 'Bond' in k: return 'B'
    return 'E'

def main():
    with open(CSV_PATH, 'r', encoding='utf-8') as f:
        rows = list(csv.DictReader(f))

    try:
        with open(SQL_PATH, 'r', encoding='utf-8') as f:
            sql_lines = f.readlines()
        sql_enc = 'utf-8'
    except UnicodeDecodeError:
        with open(SQL_PATH, 'r', encoding='cp950') as f:
            sql_lines = f.readlines()
        sql_enc = 'cp950'

    # 用第 14 行（C9）當範本，取出 "Insert into ... values (" 之前與 ");" 之後，只替換 values 內容
    template_line = sql_lines[13]  # 0-based, line 14
    if "values (" not in template_line:
        template_line = sql_lines[14]
    prefix = template_line.split(" values (")[0] + " values ("
    suffix = ");"
    # 範本 values 內容（C9 那筆）
    tmp = template_line.split(" values (", 1)[1]
    template_vals = tmp.rsplit(");", 1)[0]

    start = 12
    new_lines = []
    for i, r in enumerate(rows[start:], start=start):
        fund_no = f'C{i+1}'
        name = esc(col(r, '基金中文名稱'))
        sname = esc(col(r, '基金簡稱'))
        ename = esc(col(r, '基金英文名稱'))
        ssname = esc(col(r, '基金簡簡稱'))
        div_desc = esc(col(r, '配息說明(前台查詢用)'))
        fid = esc(col(r, 'Fund Code'))
        isin = esc(col(r, 'ISIN Code'))
        inception = parse_date(col(r, '成立日'))
        mf_rate = col(r, '經理費率') or '0'
        fee_rate = col(r, '保管費率') or '0'
        sf_rate = col(r, '分銷費率') or '0'
        of_rate = col(r, '其他費用率') or '0'
        curr = currency_map(col(r, '幣別'))
        share_class = esc(col(r, '級別'))
        cal_code = esc(col(r, '收益分配設定'))
        ac_name = esc(col(r, '基金淨值日設定'))
        min_init = col(r, '首次申購原幣下限') or '10000'
        min_amt = col(r, '再次申購原幣下限') or '1000'
        area = area_map(col(r, '投資區域'))
        ftype = fund_type_map(col(r, '基金種類2'))
        risk = col(r, '風險收益等級') or 'RR3'
        fund_master = col(r, '買回基金主帳戶')
        fund_master = f"'{esc(fund_master)}'" if fund_master else 'null'

        v = template_vals
        v = re.sub(r"^'C9'", f"'{fund_no}'", v, count=1)
        v = re.sub(r"(,'(?:[^']|'')*','(?:[^']|'')*')", f",N'{name}',N'{sname}'", v, count=1)
        v = re.sub(r",'CIEMDBUSD',", f",'{fid}',", v, count=1)
        v = re.sub(r",to_date\('2007-07-24 00:00:00','YYYY-MM-DD HH24:MI:SS'\),", f",{inception},", v, count=1)
        v = re.sub(r",1\.4,'N'", f",{mf_rate},'N'", v, count=1)
        v = re.sub(r"'Capital Group Emerging Markets Debt Fund \(LUX\) B \(USD\)'", f"N'{ename}'", v, count=1)
        v = re.sub(r"'·s¿³¥«³õ¶Å¨éBUSD'", f"N'{ssname}'", v, count=1)
        v = re.sub(r",1000,null,", f",{min_amt},null,", v, count=1)
        v = re.sub(r",10000,null,", f",{min_init},null,", v, count=1)
        v = re.sub(r"'CG6','2','B','LU0292261301'", f"'{cal_code}','2','{share_class}','{isin}'", v, count=1)
        v = re.sub(r"'CAPITAL INTERNATIONAL FUND- SUB-USD','cg6'", f"N'{ac_name}','{ac_name}'", v, count=1)
        v = re.sub(r"'RR3','S',null,null,'AC21'", f"'{risk}','S',null,null,'AC21'", v, count=1)
        v = re.sub(r"'Y','C6','\(¥»°òª÷¦³¬Û·í¤ñ­«§ë¸ê©ó«D§ë¸êµ¥¯Å¤§°ª­·ÀI¶Å¨é\)','USD'", f"'Y','{cal_code}'," + (f"N'{div_desc}'" if div_desc else "null") + f",'{curr}'", v, count=1)
        v = re.sub(r",0\.12,0,0,'0'", f",{fee_rate},0,0,'0'", v, count=1)
        v = re.sub(r"'GB'", f"'{area}'", v, count=1)
        v = re.sub(r"'USD'", f"'{curr}'", v, count=2)  # CURRENCY 與 GLOBAL_CURRENCY
        v = re.sub(r"'B'", f"'{ftype}'", v, count=2)   # FUND_TYPE 與後面一處
        v = re.sub(r"'LU810670006550196505',null,null,null,5", f"{fund_master},null,null,null,5", v, count=1)

        new_lines.append(prefix + v + suffix)

    with open(SQL_PATH, 'a', encoding=sql_enc) as f:
        f.write('\n')
        f.write('\n'.join(new_lines))
    print(f'Appended {len(new_lines)} INSERT rows to {SQL_PATH}')

if __name__ == '__main__':
    main()
