# -*- coding: utf-8 -*-
"""
完全重新產生 NEW_C_FAS.FUND_INSERT.sql (Synthetic Template Version)
來源：CG基金清單all_FAS191 1.csv (所有列)
範本：人工合成 (Synthetic)，不依賴現有 SQL 檔的內容，避免繼承錯誤。
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

def parse_time(s):
    # s format: 11:00 -> to_date('1900-01-01 11:00:00', ...)
    if not s: return "null"
    if ':' in s:
        # assume HH:MM
        parts = s.split(':')
        h = parts[0].zfill(2)
        m = parts[1].zfill(2)
        return f"to_date('1900-01-01 {h}:{m}:00','YYYY-MM-DD HH24:MI:SS')"
    return "null"

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

def div_freq_map(s):
    if not s: return 'N' # Default None
    if '季' in s: return 'Q'
    if '月' in s: return 'M'
    if '年' in s: return 'A'
    if '半年' in s: return 'S'
    return 'N'

def sales_type_map(s):
    if '正常' in s: return '0'
    if '暫停' in s: return '1'
    if '未開賣' in s: return '2'
    return '0' # Default

def fund_group_map(kind2):
    k = str(kind2).strip()
    if '債券' in k: return 'OB' # Offshore Bond
    if '股票' in k: return 'OE' # Offshore Equity
    if '平衡' in k: return 'OL' # Offshore Balanced
    return 'OE' # Default

def share_class_map(s):
    if not s: return ''
    # 1. 移除括號內容 (例如 "B (美元)" -> "B")
    s = re.sub(r'\s*\(.*?\)', '', s).strip()
    
    # 2. 對照表
    mapping = {
        'Azdm': '019', 'Azdmc1': '020', 'Aadmc1': '006', 'A': '001', 'Aadm': '005',
        'Adq': '010', 'Ae': '011', 'Aedm': '012', 'B': '021', 'C': '022',
        'D': '023', 'E': '024', 'Admc1': '009', 'Andmc1': '016', 'Andm': '015',
        'Aj': '013', 'AM': '002', 'AMH': '003', 'HC': '025', 'S': '029',
        'SD': '030', 'As': '017', 'IA': '027', 'IB': '028', 'Cdm': '031',
        'T3': '038', 'Aadq': '007', 'I': '026', 'IC': '032', 'CI': '033',
        'SA': '035', 'G': '036', 'Admc3': '039', 'Adm': '008', 'T3dmc1': '037',
        'TISA': '040', 'Az': '018', 'Aa': '004', 'An': '014', 'X': '034'
    }
    
    return mapping.get(s, s) # 若無對應，回傳原值 (已縮短)

def main():
    # 1. 讀取 CSV
    with open(CSV_PATH, 'r', encoding='utf-8') as f:
        rows = list(csv.DictReader(f))

    # 2. 定義欄位列表 (從原始檔案 header 取得，這裡是硬編碼以確保順序正確)
    cols_str = "FUND_NO,NAME,SNAME,FUND_TYPE,MAX_SHARE,INCEPTION_DATE,CUSTODIAN,CO_NO,TERM_NO,ID,PAY_DAY,IS_PERIODIC,FORMULA_RED_FEE,PREV_DATE,AC_DATE,VISA_DATE,LAST_CER_NO,LAST_S_STAT_NO,LAST_P_STAT_NO,IS_POSTED,FIRST_RED_DATE,MF_RATE,IS_EC,IS_VC,BEHIND_DAYS,IS_CCC,IS_SSS,ORDINAL,ENAME,SSNAME,CUSTODIAN_RECIPIENT,CUSTODIAN_FAX,MIN_AMOUNT,MAX_CHANGE,TAX_NO,MEDIA_NO,VAT,CTCB_FUND_NO,CTCB_AC_CODE,PERIOD_MIN,CLEAR_DATE,UWCB_FUND_NO,UWCB_AC_CODE,IS_EC_PERIODIC,EC_PERIOD_MIN,EC_MIN_AMOUNT,CUSTODIAN_BANK_NO,TW_RATE,VARIABLE_MF,NAV_DECIMAL,EMP_NO,LAST_MODIFIED,AREA_TYPE,IS_MAX_SHARE,BANK_NO,BRANCH_NO,CURRENCY_NO,FUND_TYPE2,OFFERING_TYPE,AMT_DECIMAL,SHARE_DECIMAL,MIN_RED_SHARE,MIN_BAL_SHARE,PUR_CUT_OFF,RED_CUT_OFF,CALENDAR_CODE,FUND_CATEGORY,SHARE_CLASS,ISIN_CODE,AMC_NO,RED_NAV_DAY,MIN_BAL_AMOUNT,EC_DIFF,EC_DEDUCTION,EC_REMITTANCE,EC_ATM,DEXIA_CODE,FEE_RATE,RESET_PERIOD,EARLY_RED_MIN_DAYS,EARLY_RED_FEE_RATE,IS_PERFORMANCE_FEE,ANNUALIZED_ROI,ROUNDING_SHARE,T0_CODE,HSBC_CODE,MIN_INITIAL_PURCHASE,MIN_RED_AMOUNT,PUR_NAV_DAY,IS_EC_PURCHASE,IS_EC_REDEMPTION,EC_PUR_CUT_OFF,EC_RED_CUT_OFF,LAUNCH_DATE,INVESTMENT_LINK,DIVIDEND_FREQ,AC_NAME,TX_CALENDAR_CODE,MIN_INITIAL_PURCHASE_NTD,MIN_AMOUNT_NTD,PERIOD_MIN_NTD,MIN_RED_AMOUNT_NTD,MIN_BAL_AMOUNT_NTD,EC_PERIOD_MIN_NTD,EC_MIN_AMOUNT_NTD,REDEMPTION_RULE,IS_EC_SWITCH_IN,IS_WIRE_FEE,RISK_CATEGORY,CORE_SATELLITE,TDCC_CODE,END_COLLECTION_DATE,SITCA_FUND_TYPE,ELIMINATION_DATE,EC_RSP_NEW,EC_RSP_UPDATE,FUND_GROUP,DIVIDEND_RATE,DIVIDEND_MIN,RSP_CHANGE,FAX_PUR_CUT_OFF,FUND_SET,TSCD_NAV_UPLOAD_TYPE,FUND_MASTER_NO,DIVIDEND_DESC,GLOBAL_CURRENCY,HEDGING_TYPE,CREATED_BY,CREATION_DATE,REVIEWED_BY,REVIEW_DATE,STATUS,IS_EC_DRSP,EC_DRSP_NEW,EC_DRSP_UPDATE,RED_BANK_NO,RED_BRANCH_NO,RED_AC_CODE,DIVIDEND_BANK_NO,DIVIDEND_BRANCH_NO,DIVIDEND_AC_CODE,BANK_WIRE_FEE,IS_TDCC,TDCC_DEBIT_CUT_OFF,TDCC_REMIT_CUT_OFF,TDCC_RED_CUT_OFF,SUBTA_NO,NTD_RSP_RANGE,ORG_RSP_RANGE,INVESTMENT_TYPE,SERVICE_CHARGE_TYPE,DUE_EXCHANGE_TYPE,DUE_EXCHANGE_FUND,BEL_BASE,BEL_YEAR,BEL_EXG_DAY,SALES_DATE,OPERATION_FEE_RATE,MATURITY_DATE,IS_PURCHASE,RECEIPT_FREQ,RECEIPT_CALENDAR_CODE,RECEIPT_DAY,PUR_DISCOUNT,SF_RATE,CF_RATE,OF_RATE,SALES_TYPE,IS_MAX_SHARE_RATE1,IS_MAX_SHARE_RATE2,REGISTRATION,IS_ROBO"
    cols = [c.strip() for c in cols_str.split(",")]
    col_idx = {name: i for i, name in enumerate(cols)}
    
    # 建立預設值 Template (全 null)
    # 常數預設值
    defaults = {
        'CUSTODIAN': "'JPM J.P. Morgan SE, Luxembourg Branch'",
        'PAY_DAY': '3',
        'IS_PERIODIC': "'N'",
        'AC_DATE': "to_date('2024-11-15 00:00:00','YYYY-MM-DD HH24:MI:SS')",
        'LAST_CER_NO': '0',
        'LAST_S_STAT_NO': '0',
        'LAST_P_STAT_NO': '0',
        'IS_EC': "'N'",
        'IS_VC': "'N'",
        'BEHIND_DAYS': '1',
        'OFFERING_TYPE': "'1'",
        'AMT_DECIMAL': '2',
        'SHARE_DECIMAL': '3',
        'FUND_CATEGORY': "'2'",
        'AMC_NO': "'05'",
        'IS_EC_PURCHASE': "'N'",
        'IS_EC_REDEMPTION': "'N'",
        'IS_EC_SWITCH_IN': "'N'",
        'IS_WIRE_FEE': "'N'",
        'CORE_SATELLITE': "'S'",
        'IS_ROBO': "'N'",
        'SALES_TYPE': "'0'",
        'REGISTRATION': "'LU'",
        'SERVICE_CHARGE_TYPE': "'1'",
        'DUE_EXCHANGE_TYPE': "'N'",
        'BEL_BASE': "'3'",
        'BEL_YEAR': '0',
        'BEL_EXG_DAY': '0',
        'IS_POSTED': 'null',
        'ORDINAL': 'null',
        'MIN_RED_SHARE': '0',
        'MIN_BAL_SHARE': '0',
        'MIN_BAL_AMOUNT': '0',
        'RED_NAV_DAY': '0',
        'EC_DEDUCTION': "'N'",
        'EC_REMITTANCE': "'N'",
        'IS_TDCC': "'Y'",
        'SUBTA_NO': "'5'",
        'INVESTMENT_LINK': "'N'",
        
        # 補回必須欄位 (參考匯出2.csv D18 範例與境外基金特性)
        'CO_NO': "'10'",
        'TERM_NO': "'01'",
        'IS_CCC': "'N'",
        'IS_SSS': "'N'",
        'IS_EC_PERIODIC': "'N'",
        'VARIABLE_MF': "'N'",
        'IS_MAX_SHARE': "'N'",
        'EC_RSP_NEW': "'N'",
        'EC_RSP_UPDATE': "'N'",
        'RSP_CHANGE': "'N'",
        'TSCD_NAV_UPLOAD_TYPE': "'Y'", # 境外基金通常為 Y
        'IS_EC_DRSP': "'N'",
        'EC_DRSP_NEW': "'N'",
        'EC_DRSP_UPDATE': "'N'",
        'IS_PURCHASE': "'Y'",
    }

    new_lines = []
    # 寫入檔頭
    new_lines.append("REM INSERTING into FAS.FUND")
    new_lines.append("SET DEFINE OFF;")
    header_insert = f"Insert into FAS.FUND ({cols_str}) values ("

    for i, r in enumerate(rows):
        vals = ['null'] * len(cols)
        
        for k, v in defaults.items():
            if k in col_idx:
                vals[col_idx[k]] = v
        
        # 準備資料 (使用新表頭)
        fund_no = f'C{i+1}'
        name = esc(col(r, 'NAME')) # 基金中文名稱
        sname = esc(col(r, 'SNAME')) # 基金簡稱
        ename = esc(col(r, 'ENAME')) # 基金英文名稱
        ssname = esc(col(r, 'SSNAME')) # 基金簡簡稱
        div_desc = esc(col(r, 'DIVIDEND_DESC')) # 配息說明
        fid = esc(col(r, 'ID')) # Fund Code
        isin = esc(col(r, 'ISIN_CODE')) # ISIN Code
        inception = parse_date(col(r, 'INCEPTION_DATE')) # 成立日
        mf_rate = col(r, 'MF_RATE') or '0'
        fee_rate = col(r, 'SF_RATE') or '0' # 保管費率 mapped to SF_RATE in CSV
        sf_rate = col(r, 'CF_RATE') or '0' # 分銷費率 mapped to CF_RATE
        of_rate = col(r, 'OF_RATE') or '0' # 其他費用率
        curr = currency_map(col(r, 'CURRENCY_NO')) # 幣別
        
        # 修正: SHARE_CLASS 需去除括號並嘗試對應代碼
        share_class = share_class_map(col(r, 'SHARE_CLASS')) 
        
        # 修正: CALENDAR_CODE 應對應 'CALENDAR_CODE' (基金淨值日設定)
        cal_code = esc(col(r, 'CALENDAR_CODE'))
        if len(cal_code) > 5 or not all(ord(c) < 128 for c in cal_code):
            cal_code = ''
            
        # 修正: DIVIDEND_FREQ 應對應 'DIVIDEND_FREQ' (收益分配設定)
        div_freq_raw = col(r, 'DIVIDEND_FREQ')
        div_freq = div_freq_map(div_freq_raw)
        
        # 修正: FUND_GROUP (2 chars) logic
        fund_group = fund_group_map(col(r, 'FUND_TYPE'))
        
        ac_name = esc(col(r, 'AC_NAME')) # 基金專戶名稱
        tx_cal_code = esc(col(r, 'TX_CALENDAR_CODE')) # 基金交易日設定
        
        min_init = col(r, 'MIN_INITIAL_PURCHASE') or '10000'
        min_amt = col(r, 'MIN_AMOUNT') or '1000'
        area = area_map(col(r, 'AREA_TYPE')) # 投資區域
        ftype = fund_type_map(col(r, 'FUND_TYPE')) # 基金種類2
        risk = col(r, 'RISK_CATEGORY') or 'RR3'
        
        fund_master = col(r, 'FUND_MASTER_NO') # 主基金 (原買回基金主帳戶) -> FUND_MASTER_NO
        # 修正: 若 FUND_MASTER_NO 超過 5 碼 (例如誤填 ISIN)，則設為 null
        if fund_master and len(fund_master) > 5:
            fund_master = None
        fund_master = f"'{esc(fund_master)}'" if fund_master else 'null'
        
        # 時間欄位
        tdcc_debit = parse_time(col(r, 'TDCC_DEBIT_CUT_OFF')) # 扣款時間
        tdcc_remit = parse_time(col(r, 'TDCC_REMIT_CUT_OFF')) # 匯款時間
        tdcc_red = parse_time(col(r, 'TDCC_RED_CUT_OFF')) # 贖回時間

        # 邏輯推導欄位
        sitca = "'AA2'" if ftype == 'E' else "'AC21'"
        op_fee = fee_rate # OPERATION_FEE_RATE (保管費率)

        # 替換值
        def set_val(col_name, v):
            if col_name in col_idx:
                vals[col_idx[col_name]] = v

        set_val('FUND_NO', f"'{fund_no}'")
        set_val('NAME', f"N'{name}'")
        set_val('SNAME', f"N'{sname}'")
        set_val('ENAME', f"N'{ename}'")
        set_val('SSNAME', f"N'{ssname}'")
        
        # ID 截斷修正
        if fid and len(fid) > 10:
             fid = fid[:10]
        set_val('ID', f"'{fid}'")
        
        set_val('INCEPTION_DATE', inception)
        set_val('MF_RATE', mf_rate)
        set_val('FEE_RATE', fee_rate)
        set_val('SF_RATE', sf_rate)
        set_val('OF_RATE', of_rate)
        set_val('OPERATION_FEE_RATE', op_fee) 
        set_val('CURRENCY_NO', f"'{curr}'")
        set_val('GLOBAL_CURRENCY', f"'{curr}'")
        set_val('SHARE_CLASS', f"'{share_class}'")
        
        set_val('CALENDAR_CODE', f"'{cal_code}'")
        set_val('FUND_GROUP', f"'{fund_group}'")
        set_val('DIVIDEND_FREQ', f"'{div_freq}'")
        
        set_val('AC_NAME', f"N'{ac_name}'")
        set_val('TX_CALENDAR_CODE', f"'{tx_cal_code}'") # 使用正確的 TX_CALENDAR_CODE
        
        set_val('MIN_INITIAL_PURCHASE', min_init)
        set_val('MIN_AMOUNT', min_amt)
        set_val('AREA_TYPE', f"'{area}'")
        set_val('FUND_TYPE', f"'{ftype}'")
        set_val('INVESTMENT_TYPE', f"'{ftype}'")
        
        set_val('RISK_CATEGORY', f"'{risk}'")
        set_val('ISIN_CODE', f"'{isin}'")
        set_val('DIVIDEND_DESC', f"N'{div_desc}'" if div_desc else "null")
        set_val('FUND_MASTER_NO', fund_master)
        
        set_val('SITCA_FUND_TYPE', sitca)
        
        set_val('TDCC_DEBIT_CUT_OFF', tdcc_debit)
        set_val('TDCC_REMIT_CUT_OFF', tdcc_remit)
        set_val('TDCC_RED_CUT_OFF', tdcc_red)
        
        launch_date = parse_date(col(r, 'SALES_DATE')) # 開賣日
        set_val('LAUNCH_DATE', launch_date)

        line_val = ",".join(vals)
        new_lines.append(header_insert + line_val + ");")

    with open(SQL_PATH, 'w', encoding='utf-8') as f:
        f.write('\n'.join(new_lines))
        f.write('\n')

    print(f'Regenerated {len(rows)} INSERT rows to {SQL_PATH}')

if __name__ == '__main__':
    main()
