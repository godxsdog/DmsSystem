# -*- coding: utf-8 -*-
"""
完全重新產生 NEW_C_FAS.FUND_INSERT.sql
來源：CG基金清單all_FAS191 1.csv (所有列)
範本：取自原 SQL 檔的 C9 (第 14 行)，但會解析並替換所有變動欄位
"""
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

# 簡易 SQL Values 解析器 (處理引號與逗號)
def parse_sql_values(val_str):
    values = []
    current = []
    in_quote = False
    i = 0
    while i < len(val_str):
        c = val_str[i]
        if c == "'":
            # 檢查是否為跳脫引號 ''
            if in_quote and i + 1 < len(val_str) and val_str[i+1] == "'":
                current.append("'")
                i += 1
            else:
                in_quote = not in_quote
        elif c == ',' and not in_quote:
            values.append("".join(current))
            current = []
            i += 1
            continue
        current.append(c)
        i += 1
    values.append("".join(current))
    return values

def main():
    # 1. 讀取 CSV
    with open(CSV_PATH, 'r', encoding='utf-8') as f:
        rows = list(csv.DictReader(f))

    # 2. 讀取 SQL 以取得 Header 與 C9 範本
    # 嘗試用 UTF-8 讀取，若失敗則用 CP950 (為了容錯，雖然現在可能是壞掉的 UTF-8)
    try:
        with open(SQL_PATH, 'r', encoding='utf-8') as f:
            lines = f.readlines()
    except UnicodeDecodeError:
        with open(SQL_PATH, 'r', encoding='cp950') as f:
            lines = f.readlines()

    header_insert = ""
    template_line = ""
    
    for line in lines:
        if "Insert into FAS.FUND" in line and not header_insert:
            # 取得 insert header (直到 values 前)
            header_insert = line.split(" values (")[0] + " values ("
        if "values ('C9'" in line:
            template_line = line
            break
            
    if not template_line:
        print("Error: Could not find C9 template line.")
        return

    # 解析 C9 範本的值
    # template_line 格式: ... values ('C9', ... );
    val_part = template_line.split(" values (", 1)[1].rsplit(");", 1)[0]
    template_vals = parse_sql_values(val_part)
    
    # 取得欄位名稱列表以確定 Index
    # header_insert 格式: Insert into FAS.FUND (COL1,COL2,...) values (
    col_str = header_insert.split("(")[1].split(") values")[0]
    cols = [c.strip() for c in col_str.split(",")]
    col_idx = {name: i for i, name in enumerate(cols)}

    # 定義要替換的欄位與對應邏輯
    # 這些欄位會從 CSV 取得，其餘保留 C9 範本值
    
    new_lines = []
    # 寫入檔頭
    new_lines.append("REM INSERTING into FAS.FUND")
    new_lines.append("SET DEFINE OFF;")

    for i, r in enumerate(rows):
        # 複製一份範本值
        vals = list(template_vals)
        
        # 準備資料
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
        ac_name = esc(col(r, '基金淨值日設定')) # 其實 CSV 欄位名稱是 "基金淨值日設定" 但 script 用來填 AC_NAME
        min_init = col(r, '首次申購原幣下限') or '10000'
        min_amt = col(r, '再次申購原幣下限') or '1000'
        area = area_map(col(r, '投資區域'))
        ftype = fund_type_map(col(r, '基金種類2'))
        risk = col(r, '風險收益等級') or 'RR3'
        fund_master = col(r, '買回基金主帳戶')
        fund_master = f"'{esc(fund_master)}'" if fund_master else 'null'

        # 替換值 (使用 col_idx 查找位置)
        def set_val(col_name, v):
            if col_name in col_idx:
                vals[col_idx[col_name]] = v

        set_val('FUND_NO', f"'{fund_no}'")
        set_val('NAME', f"N'{name}'")
        set_val('SNAME', f"N'{sname}'")
        set_val('ENAME', f"N'{ename}'")
        set_val('SSNAME', f"N'{ssname}'")
        set_val('ID', f"'{fid}'")
        set_val('INCEPTION_DATE', inception)
        set_val('MF_RATE', mf_rate)
        set_val('FEE_RATE', fee_rate)
        set_val('SF_RATE', sf_rate) # 原 script 沒換? 補上
        set_val('OF_RATE', of_rate) # 原 script 沒換? 補上
        set_val('CURRENCY_NO', f"'{curr}'")
        set_val('GLOBAL_CURRENCY', f"'{curr}'")
        set_val('SHARE_CLASS', f"'{share_class}'")
        set_val('CALENDAR_CODE', f"'{cal_code}'")
        set_val('FUND_GROUP', f"'{cal_code}'") # Gen script mapped logic
        set_val('AC_NAME', f"N'{ac_name}'")
        set_val('TX_CALENDAR_CODE', f"'{ac_name}'") # Gen script mapped logic
        set_val('MIN_INITIAL_PURCHASE', min_init)
        set_val('MIN_AMOUNT', min_amt)
        set_val('AREA_TYPE', f"'{area}'")
        set_val('FUND_TYPE', f"'{ftype}'")
        set_val('INVESTMENT_TYPE', f"'{ftype}'")
        
        set_val('RISK_CATEGORY', f"'{risk}'")
        set_val('ISIN_CODE', f"'{isin}'")
        set_val('DIVIDEND_DESC', f"N'{div_desc}'" if div_desc else "null")
        set_val('FUND_MASTER_NO', fund_master)

        # 組裝 Values String
        # 注意：vals 裡面的字串已經包含需要的引號 (例如 "'C1'") 或 to_date(...)
        # parse_sql_values 讀進來時去除了外層引號嗎? 不，parse_sql_values 保留內容。
        # 範本內容 'C9' -> 解析為 'C9' (含引號)。
        # 因此 set_val 時也要給含引號的字串。
        
        # 修正 parse_sql_values 的行為：
        # 我的 parse_sql_values 實作會保留內容，所以 'C9' 會被讀成 'C9'。
        # 因此 set_val 正確。
        
        line_val = ",".join(vals)
        new_lines.append(header_insert + line_val + ");")

    with open(SQL_PATH, 'w', encoding='utf-8') as f:
        f.write('\n'.join(new_lines))
        f.write('\n') # EOF newline

    print(f'Regenerated {len(rows)} INSERT rows to {SQL_PATH}')

if __name__ == '__main__':
    main()
