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
CSV_PATH = os.path.join(BASE, 'CG基金清單all_FAS191.csv')
SQL_PATH = os.path.join(BASE, 'NEW_C_FAS.FUND_INSERT.sql')

def col(d, k):
    """從 CSV 行中讀取欄位，支援中英文欄位名稱映射"""
    if not isinstance(d, dict):
        return ''
    
    # 中文到英文的欄位名稱映射
    chinese_to_english = {
        '基金代號': 'FUND_NO',
        '基金中文名稱': 'NAME',
        '基金簡稱': 'SNAME',
        '基金英文名稱': 'ENAME',
        '基金簡簡稱': 'SSNAME',
        'Fund Code': 'HSBC_CODE',  # Fund Code 映射到 HSBC_CODE
        'ISIN Code': 'ISIN_CODE',
        '基金經理公司': 'AMC_NO',
        '基金專戶名稱': 'AC_NAME',
        '基金保管機構': 'CUSTODIAN',
        '基金註冊地': 'REGISTRATION',
        '避險級別': 'HEDGING_TYPE',
        '全球資產公告幣別': 'GLOBAL_CURRENCY',
        '銷售狀態': 'SALES_TYPE',
        '經理費率': 'MF_RATE',
        '變動經理費率': 'VARIABLE_MF',
        '保管費率': 'SF_RATE',
        '分銷費率': 'CF_RATE',
        '其他費用率': 'OF_RATE',
        '募集方式': 'OFFERING_TYPE',
        '投資區域': 'AREA_TYPE',
        '幣別': 'CURRENCY_NO',
        '級別': 'SHARE_CLASS',
        '風險收益等級': 'RISK_CATEGORY',
        '基金種類2': 'FUND_TYPE',
        '投資類型': 'INVESTMENT_TYPE',
        '投信投顧公會基金類型': 'SITCA_FUND_TYPE',
        '衛星基金': 'CORE_SATELLITE',
        '收益分配設定': 'DIVIDEND_FREQ',
        '基金淨值日設定': 'CALENDAR_CODE',
        '基金交易日設定': 'TX_CALENDAR_CODE',
        '成立日': 'INCEPTION_DATE',
        '開賣日': 'SALES_DATE',
        '開始買回日': 'FIRST_RED_DATE',
        '開放集保NAV上傳': 'TSCD_NAV_UPLOAD_TYPE',
        '申購淨值日': 'PUR_NAV_DAY',
        '買回淨值日': 'RED_NAV_DAY',
        '買回付款日': 'PAY_DAY',
        '記帳日落差天數': 'BEHIND_DAYS',
        'EC交易日落差天數': 'EC_DIFF',
        '買回基金主帳戶銀行': 'RED_BANK_NO',
        '收益分配配息專戶銀行': 'DIVIDEND_BANK_NO',
        '開放集保': 'IS_TDCC',
        '手續費收取時點': 'SERVICE_CHARGE_TYPE',
        '首次申購原幣下限': 'MIN_INITIAL_PURCHASE',
        '再次申購原幣下限': 'MIN_AMOUNT',
        '收益分配下限': 'DIVIDEND_MIN',
        '結存原幣下限': 'MIN_BAL_AMOUNT',
        '淨值小數位數': 'NAV_DECIMAL',
        '價金小數位數': 'AMT_DECIMAL',
        '單位數小數位數': 'SHARE_DECIMAL',
        '單位數捨位方式': 'ROUNDING_SHARE',
        '買回價金計算方式': 'REDEMPTION_RULE',
        '客戶支付買回匯費': 'IS_WIRE_FEE',
        '買回分行': 'RED_BRANCH_NO',
        '受益分配分行': 'DIVIDEND_BRANCH_NO',
        '扣款時間': 'TDCC_DEBIT_CUT_OFF',
        '短線交易最小持有天數': 'EARLY_RED_MIN_DAYS',
        '是否收取買回費用': 'FORMULA_RED_FEE',
        '是否收取績效管理費': 'IS_PERFORMANCE_FEE',
        '是否年化報酬率': 'ANNUALIZED_ROI',
        '匯款/ATM銀行': 'BANK_NO',
        '匯款分行代碼': 'BRANCH_NO',
        '申購關帳時間': 'PUR_CUT_OFF',
        '買回關帳時間': 'RED_CUT_OFF',
        'EC及RSP申購關帳時間': 'EC_PUR_CUT_OFF',
        'EC買回關帳時間': 'EC_RED_CUT_OFF',
        '買回基金主帳戶': 'RED_AC_CODE',
        '收益分配配息專戶': 'DIVIDEND_AC_CODE',
        '匯款時間': 'TDCC_REMIT_CUT_OFF',
        '開放投資型連結保單': 'INVESTMENT_LINK',
        '開放申購': 'IS_PURCHASE',
        '開放定期定額': 'IS_PERIODIC',
        '開放語音交易': 'IS_VC',
        '開放網路交易': 'IS_EC',
        '開放網路申購': 'IS_EC_PURCHASE',
        'EC變更書面RSP': 'RSP_CHANGE',
        '開放網路轉申購': 'IS_EC_SWITCH_IN',
        '開放網路RSP': 'IS_EC_PERIODIC',
        '開放網路DRSP': 'IS_EC_DRSP',
        '開放ATM轉帳': 'EC_ATM',
        '開放網路買回': 'IS_EC_REDEMPTION',
        '開放銀行扣款': 'EC_DEDUCTION',
        '開放銀行匯款': 'EC_REMITTANCE',
        '開放ROBO': 'IS_ROBO',
        '匯費': 'BANK_WIRE_FEE',
        '登錄代理人代號': 'SUBTA_NO',
        '贖回時間': 'TDCC_RED_CUT_OFF',
        '主基金': 'FUND_MASTER_NO',
        '配息說明(前台查詢用)': 'DIVIDEND_DESC',
        'No.': 'ORDINAL',
        'EC及RSP申購': 'EC_RSP_NEW',
        'EC及RSP更新': 'EC_RSP_UPDATE',
    }
    
    # 先嘗試直接讀取，如果失敗則嘗試中文映射
    value = d.get(k, '')
    if not value and k in chinese_to_english.values():
        # k 是英文，反向查找中文
        for cn, en in chinese_to_english.items():
            if en == k:
                value = d.get(cn, '')
                break
    
    return value.strip() if value else ''

def yn_map(s):
    """將中文是/否轉換為 Y/N"""
    if not s:
        return ''
    s = s.strip()
    if s in ['是', 'Y', 'y', 'YES', 'yes']:
        return 'Y'
    if s in ['否', 'N', 'n', 'NO', 'no']:
        return 'N'
    return s

def redemption_rule_map(s):
    """贖回計價方式對應"""
    if not s:
        return 'C'
    if '總單位數' in s or '價金' in s:
        return 'C'
    if '先進先出' in s or 'FIFO' in s:
        return 'F'
    return 'C'

def core_satellite_map(s):
    """核心/衛星對應 (CSV 中 "是" 表示衛星基金)"""
    if not s:
        return 'S'
    s = s.strip()
    # 根據 D18 範例與 CSV 實際情況，"是" 對應 S (衛星)
    if s in ['是', 'S', 'Satellite', '衛星']:
        return 'S'
    if s in ['否', 'C', 'Core', '核心']:
        return 'C'
    return 'S'

def offering_type_map(s):
    """募集類型對應"""
    if not s:
        return '1'
    if '公開募集' in s or 'public' in s.lower():
        return '1'
    if '私募' in s or 'private' in s.lower():
        return '2'
    return s if s in ['1', '2'] else '1'

def service_charge_map(s):
    """收費類型對應"""
    if not s:
        return '1'
    if '前收' in s or 'front' in s.lower():
        return '1'
    if '後收' in s or 'back' in s.lower():
        return '2'
    return s if s in ['1', '2'] else '1'

def sales_type_map(s):
    """銷售類型對應"""
    if not s:
        return '0'
    if '正常銷售' in s or 'normal' in s.lower():
        return '0'
    return s if s in ['0', '1', '2'] else '0'

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

def clean_share_class(s):
    """清理級別字段：只保留英文字母和數字，刪除中文、括號、空格等"""
    if not s:
        return ''
    import re
    # 只保留英文字母 (a-zA-Z) 和數字 (0-9)
    cleaned = re.sub(r'[^a-zA-Z0-9]', '', str(s))
    return cleaned.strip()

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

def fund_type2_map(fund_type):
    """推導 FUND_TYPE2 (基金種類 + 區域代碼)"""
    # 境外基金通常加 9，例如 E9, B9
    if fund_type == 'E':
        return 'E9'
    if fund_type == 'B':
        return 'B9'
    return f'{fund_type}9'

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
    # 常數預設值 (參考 D18 範例資料)
    defaults = {
        'CUSTODIAN': "'JPM J.P. Morgan SE, Luxembourg Branch'",
        'CO_NO': "'10'",
        'TERM_NO': "'01'",
        'ID': "''",  # 總受益憑證ID 應該是空的
        'PAY_DAY': '5', # D18: 5
        'BEHIND_DAYS': '1',
        'AC_DATE': "to_date('2025-12-31 00:00:00','YYYY-MM-DD HH24:MI:SS')",  # 結帳日統一為 2025/12/31
        'LAST_CER_NO': '0', # D18: 345, 但這是流水號，設為 0
        'LAST_S_STAT_NO': '0', # D18: 200522, 但這是流水號，設為 0
        'LAST_P_STAT_NO': '0', # D18: 124035, 但這是流水號，設為 0
        'IS_POSTED': "'P'", # D18: P (已過帳)
        'ORDINAL': 'null', # D18: 18, 但這是序號，維持 null
        'VAT': 'null',  # VAT 應該是空的
        'MAX_SHARE': 'null',  # MAX_SHARE 應該是空的
        'MEDIA_NO': 'null',  # MEDIA_NO 應該是空的
        'UWCB_FUND_NO': "''",  # UWCB_FUND_NO 應該是空的
        'BANK_NO': "''",  # BANK_NO 應該是空的
        'BRANCH_NO': "''",  # BRANCH_NO 應該是空的
        'FEE_RATE': 'null',  # FEE_RATE 應該是空的
        'EARLY_RED_FEE_RATE': 'null',  # EARLY_RED_FEE_RATE 應該是空的
        
        # 費率與小數位數
        'AMT_DECIMAL': '0', # D18: 0
        'NAV_DECIMAL': '2', # D18: 2
        'FUND_CATEGORY': "'2'", # 境外基金 (修改為 2)
        'OFFERING_TYPE': "'1'", # D18: 1
        'MIN_RED_SHARE': '0',
        'MIN_BAL_SHARE': '300', # D18: 300
        'PERIOD_MIN': '3000', # D18: 3000
        'EC_PERIOD_MIN': '3000', # D18: 3000
        'EC_MIN_AMOUNT': '3000', # D18: 3000
        
        # 銀行資訊 (可從 CSV 讀取或使用預設值)
        'CUSTODIAN_BANK_NO': "'007'", # D18: 007
        # BANK_NO, BRANCH_NO 已在前面設置為空
        'RED_BANK_NO': "'007'", # D18: 007
        
        # 文字欄位預設值
        'CUSTODIAN_FAX': "'2382-0511'", # D18 範例值
        'CUSTODIAN_RECIPIENT': "''", # 境外基金通常不需要聯絡人
        'EMP_NO': "'A00907'", # D18 範例值
        # VAT, MEDIA_NO, UWCB_FUND_NO, BANK_NO, BRANCH_NO, FEE_RATE, EARLY_RED_FEE_RATE 已在前面設置為空
        'STATUS': 'null', # 用戶要求設為 null
        'REVIEWED_BY': "'A01096'", # D18 範例審核人員
        'FUND_SET': "'05'", # D18: 05
        'TDCC_CODE': "''", # 境外基金集保代碼（IS_TDCC='N'時設為空）
        'REDEMPTION_RULE': "'C'", # D18: C
        'ROUNDING_SHARE': "'O'", # D18: O
        'T0_CODE': "''", # D18: 4F0DIO02, 但每檔不同，設為空
        # UWCB_FUND_NO 已在前面設置為空
        'UWCB_AC_CODE': "''", # D18: 015016020912, 但可能每檔不同
        
        # 費率
        'TW_RATE': '0.0175', # D18: 0.0175
        'EARLY_RED_MIN_DAYS': '7', # D18: 7
        # EARLY_RED_FEE_RATE 已在前面設置為 null
        'IS_MAX_SHARE_RATE1': '0.6', # D18: 0.6
        'IS_MAX_SHARE_RATE2': '0.7', # D18: 0.7
        
        # 金額範圍
        'NTD_RSP_RANGE': '1000', # D18: 1000
        'ORG_RSP_RANGE': '1000', # D18: 1000
        
        # 其他常數
        'BEL_BASE': "'3'",
        'BEL_YEAR': '0',
        'BEL_EXG_DAY': '0',
        'MAX_CHANGE': '7', # D18: 7
        # MAX_SHARE 已在前面設置為 null
        
        # Y/N 布林欄位 (依 D18 範例設定)
        'IS_PERIODIC': "'Y'", # D18: Y
        'IS_EC': "'Y'", # D18: Y
        'IS_VC': "'N'",
        'IS_CCC': "'Y'", # D18: Y
        'IS_SSS': "'Y'", # D18: Y
        'IS_EC_PERIODIC': "'Y'", # D18: Y
        'VARIABLE_MF': "'N'",
        'IS_MAX_SHARE': "'Y'", # D18: Y
        'IS_EC_PURCHASE': "'Y'", # D18: Y
        'IS_EC_REDEMPTION': "'Y'", # D18: Y
        'IS_EC_SWITCH_IN': "'Y'", # D18: Y
        'IS_WIRE_FEE': "'Y'", # D18: Y
        'EC_DEDUCTION': "'Y'", # D18: Y
        'EC_REMITTANCE': "'N'",
        'EC_ATM': "'N'",
        'IS_TDCC': "'N'", # D18: N (境外基金不用集保)
        'INVESTMENT_LINK': "'Y'", # D18: Y
        'IS_PERFORMANCE_FEE': "'N'",
        'ANNUALIZED_ROI': "'N'",
        'HEDGING_TYPE': "'N'",
        'EC_RSP_NEW': "'Y'", # D18: Y
        'EC_RSP_UPDATE': "'Y'", # D18: Y
        'RSP_CHANGE': "'Y'", # D18: Y
        'TSCD_NAV_UPLOAD_TYPE': "'N'", # D18: N (注意與之前 Y 不同)
        'IS_EC_DRSP': "'Y'", # D18: Y
        'EC_DRSP_NEW': "'Y'", # D18: Y
        'EC_DRSP_UPDATE': "'Y'", # D18: Y
        'IS_PURCHASE': "'Y'",
        'IS_ROBO': "'N'",
        'DUE_EXCHANGE_TYPE': "'N'",
        'DIVIDEND_FREQ': "'N'", # D18: N
        
        # 其他欄位
        'SUBTA_NO': "'01'", # D18: 01 (非 5)
        'SERVICE_CHARGE_TYPE': "'1'",
        'SALES_TYPE': "'0'",
        'REGISTRATION': "'TW'", # D18: TW (注意與之前 LU 不同，應從 CSV 讀取)
        'PUR_NAV_DAY': '0', # D18: 0
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
        fund_no = col(r, 'FUND_NO') or f'G{i+1}'  # 從 CSV 的「基金代號」讀取
        name = esc(col(r, 'NAME')) # 基金中文名稱
        sname = esc(col(r, 'SNAME')) # 基金簡稱
        ename = esc(col(r, 'ENAME')) # 基金英文名稱
        ssname = esc(col(r, 'SSNAME')) # 基金簡簡稱
        div_desc = esc(col(r, 'DIVIDEND_DESC')) # 配息說明
        # ID (總受益憑證ID) 應該是空的，不使用 Fund Code
        isin = esc(col(r, 'ISIN_CODE')) # ISIN Code
        inception = parse_date(col(r, 'INCEPTION_DATE')) # 成立日
        mf_rate = col(r, 'MF_RATE') or '0'
        fee_rate = col(r, 'SF_RATE') or '0' # 保管費率 mapped to SF_RATE in CSV
        sf_rate = col(r, 'CF_RATE') or '0' # 分銷費率 mapped to CF_RATE
        of_rate = col(r, 'OF_RATE') or '0' # 其他費用率
        curr = currency_map(col(r, 'CURRENCY_NO')) # 幣別
        
        # 從 CSV 讀取更多欄位 (如果有值就覆蓋預設值，使用映射函數處理中文)
        registration = col(r, 'REGISTRATION') or 'LU'
        share_decimal = col(r, 'SHARE_DECIMAL') or '3'
        amt_decimal = col(r, 'AMT_DECIMAL') or '2'
        nav_decimal = col(r, 'NAV_DECIMAL') or '2'
        offering_type = offering_type_map(col(r, 'OFFERING_TYPE'))
        service_charge_type = service_charge_map(col(r, 'SERVICE_CHARGE_TYPE'))
        sales_type = sales_type_map(col(r, 'SALES_TYPE'))
        pay_day = col(r, 'PAY_DAY') or '5'
        behind_days = col(r, 'BEHIND_DAYS') or '1'
        early_red_min = col(r, 'EARLY_RED_MIN_DAYS') or '7'
        
        # SHARE_CLASS 清理級別字段：只保留英文字母和數字，刪除中文、括號、空格
        share_class = esc(clean_share_class(col(r, 'SHARE_CLASS'))) 
        
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
        min_bal_amt = col(r, 'MIN_BAL_AMOUNT') or '0'
        dividend_min = col(r, 'DIVIDEND_MIN') or 'null'
        area = area_map(col(r, 'AREA_TYPE')) # 投資區域
        ftype = fund_type_map(col(r, 'FUND_TYPE')) # 基金種類2
        risk = col(r, 'RISK_CATEGORY') or 'RR3'
        core_sat = core_satellite_map(col(r, 'CORE_SATELLITE')) # 使用映射函數
        redemption_rule = redemption_rule_map(col(r, 'REDEMPTION_RULE'))
        
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
        
        # ID (總受益憑證ID) 保持為空，不設置值
        
        set_val('INCEPTION_DATE', inception)
        set_val('MF_RATE', mf_rate)
        # FEE_RATE 保持為 null，不設置值
        set_val('SF_RATE', sf_rate)
        set_val('CF_RATE', sf_rate)  # CF_RATE 使用與 SF_RATE 相同的值
        set_val('OF_RATE', of_rate)
        set_val('OPERATION_FEE_RATE', op_fee) 
        set_val('CURRENCY_NO', f"'{curr}'")
        set_val('GLOBAL_CURRENCY', f"'{curr}'")
        set_val('SHARE_CLASS', f"'{share_class}'")
        
        # 從 CSV 讀取的數字與文字欄位
        set_val('REGISTRATION', f"'{registration}'")
        set_val('SHARE_DECIMAL', share_decimal)
        set_val('AMT_DECIMAL', amt_decimal)
        set_val('NAV_DECIMAL', nav_decimal)
        set_val('OFFERING_TYPE', f"'{offering_type}'")
        set_val('SERVICE_CHARGE_TYPE', f"'{service_charge_type}'")
        set_val('SALES_TYPE', f"'{sales_type}'")
        set_val('PAY_DAY', pay_day)
        set_val('BEHIND_DAYS', behind_days)
        set_val('EARLY_RED_MIN_DAYS', early_red_min)
        
        set_val('CALENDAR_CODE', f"'{cal_code}'")
        set_val('FUND_GROUP', f"'{fund_group}'")
        set_val('DIVIDEND_FREQ', f"'{div_freq}'")
        
        # 從 CSV 讀取所有 Y/N 布林欄位 (將中文轉換為 Y/N)
        yn_fields = ['IS_PERIODIC', 'IS_EC', 'IS_VC', 'IS_CCC', 'IS_SSS', 
                     'IS_EC_PERIODIC', 'VARIABLE_MF', 'IS_MAX_SHARE', 
                     'IS_EC_PURCHASE', 'IS_EC_REDEMPTION', 'IS_EC_SWITCH_IN', 
                     'IS_WIRE_FEE', 'EC_DEDUCTION', 'EC_REMITTANCE', 'EC_ATM',
                     'IS_TDCC', 'INVESTMENT_LINK', 'IS_PERFORMANCE_FEE', 
                     'ANNUALIZED_ROI', 'HEDGING_TYPE', 'EC_RSP_NEW', 'EC_RSP_UPDATE',
                     'RSP_CHANGE', 'TSCD_NAV_UPLOAD_TYPE', 'IS_EC_DRSP', 
                     'EC_DRSP_NEW', 'EC_DRSP_UPDATE', 'IS_PURCHASE', 'IS_ROBO',
                     'DUE_EXCHANGE_TYPE']
        
        for yn_field in yn_fields:
            val = yn_map(col(r, yn_field))  # 使用 yn_map 轉換中文
            if val in ['Y', 'N']:
                set_val(yn_field, f"'{val}'")
        
        set_val('AC_NAME', f"N'{ac_name}'")
        set_val('TX_CALENDAR_CODE', f"'{tx_cal_code}'") # 使用正確的 TX_CALENDAR_CODE
        
        set_val('MIN_INITIAL_PURCHASE', min_init)
        set_val('MIN_AMOUNT', min_amt)
        set_val('MIN_BAL_AMOUNT', min_bal_amt)
        if dividend_min != 'null':
            set_val('DIVIDEND_MIN', dividend_min)
        else:
            set_val('DIVIDEND_MIN', 'null')
        set_val('AREA_TYPE', f"'{area}'")
        set_val('FUND_TYPE', f"'{ftype}'")
        set_val('INVESTMENT_TYPE', f"'{ftype}'")
        set_val('FUND_TYPE2', f"'{fund_type2_map(ftype)}'")
        
        set_val('RISK_CATEGORY', f"'{risk}'")
        set_val('CORE_SATELLITE', f"'{core_sat}'")
        set_val('ISIN_CODE', f"'{isin}'")
        set_val('DIVIDEND_DESC', f"N'{div_desc}'" if div_desc else "null")
        set_val('FUND_MASTER_NO', fund_master)
        
        set_val('SITCA_FUND_TYPE', sitca)
        
        set_val('TDCC_DEBIT_CUT_OFF', tdcc_debit)
        set_val('TDCC_REMIT_CUT_OFF', tdcc_remit)
        set_val('TDCC_RED_CUT_OFF', tdcc_red)
        
        launch_date = parse_date(col(r, 'SALES_DATE')) # 開賣日
        set_val('LAUNCH_DATE', launch_date)
        
        # 補上其他從 CSV 讀取的欄位
        # 從 CSV 讀取數字欄位（如有值則覆蓋預設值）
        
        # ORDINAL: 根據索引從 1000 開始 (G1=1000, G2=1001, G3=1002...)
        set_val('ORDINAL', str(1000 + i))
        
        # HSBC_CODE: 讀取 Fund Code
        hsbc_code = esc(col(r, 'HSBC_CODE'))  # Fund Code
        if hsbc_code:
            set_val('HSBC_CODE', f"'{hsbc_code}'")
        else:
            set_val('HSBC_CODE', "null")
        
        if col(r, 'AMC_NO'):
            amc_no = col(r, 'AMC_NO')
            if amc_no and amc_no.isdigit():
                amc_no = amc_no.zfill(2)
            set_val('AMC_NO', f"'{amc_no}'")
        # MAX_SHARE 保持為 null，不從 CSV 讀取
        if col(r, 'EC_DIFF'):
            ec_diff = col(r, 'EC_DIFF')
            set_val('EC_DIFF', ec_diff if ec_diff else '0')
        if col(r, 'TDCC_CODE'):
            set_val('TDCC_CODE', f"'{col(r, 'TDCC_CODE')}'")
        if col(r, 'RED_NAV_DAY'):
            set_val('RED_NAV_DAY', col(r, 'RED_NAV_DAY'))
        # BANK_NO, BRANCH_NO 保持為空，不從 CSV 讀取
        if col(r, 'RED_BRANCH_NO'):
            red_branch = col(r, 'RED_BRANCH_NO')
            # 修正: RED_BRANCH_NO 最大 4 字元，若超過則設為空
            if len(red_branch) > 4:
                red_branch = ''
            set_val('RED_BRANCH_NO', f"'{red_branch}'")
        if col(r, 'DIVIDEND_BANK_NO'):
            div_bank = col(r, 'DIVIDEND_BANK_NO')
            # 修正: DIVIDEND_BANK_NO 最大 3 字元，若超過則設為空（參考 D18）
            if len(div_bank) > 3:
                div_bank = ''
            set_val('DIVIDEND_BANK_NO', f"'{div_bank}'")
        if col(r, 'DIVIDEND_BRANCH_NO'):
            div_branch = col(r, 'DIVIDEND_BRANCH_NO')
            # 修正: DIVIDEND_BRANCH_NO 最大 4 字元，若超過則設為空
            if len(div_branch) > 4:
                div_branch = ''
            set_val('DIVIDEND_BRANCH_NO', f"'{div_branch}'")
        if col(r, 'RED_AC_CODE'):
            red_ac = col(r, 'RED_AC_CODE')
            # 修正: RED_AC_CODE 如果太長（ISIN），截斷或設為空
            if len(red_ac) > 20:
                red_ac = red_ac[:20]
            set_val('RED_AC_CODE', f"'{red_ac}'" if red_ac else 'null')
        if col(r, 'DIVIDEND_AC_CODE'):
            set_val('DIVIDEND_AC_CODE', f"'{col(r, 'DIVIDEND_AC_CODE')}'")
        if col(r, 'ROUNDING_SHARE'):
            set_val('ROUNDING_SHARE', f"'{col(r, 'ROUNDING_SHARE')}'")
        
        # 使用映射函數
        set_val('REDEMPTION_RULE', f"'{redemption_rule}'")
        
        if col(r, 'PUR_NAV_DAY'):
            set_val('PUR_NAV_DAY', col(r, 'PUR_NAV_DAY'))
        if col(r, 'EARLY_RED_MIN_DAYS'):
            set_val('EARLY_RED_MIN_DAYS', col(r, 'EARLY_RED_MIN_DAYS'))
        if col(r, 'SUBTA_NO'):
            set_val('SUBTA_NO', f"'{col(r, 'SUBTA_NO')}'")
        if col(r, 'BANK_WIRE_FEE'):
            set_val('BANK_WIRE_FEE', col(r, 'BANK_WIRE_FEE'))
        if col(r, 'MIN_RED_AMOUNT'):
            set_val('MIN_RED_AMOUNT', col(r, 'MIN_RED_AMOUNT'))
        if col(r, 'REGISTRATION'):
            set_val('REGISTRATION', f"'{col(r, 'REGISTRATION')}'")
        
        # RED_BANK_NO: CSV 中可能是保管機構名稱而非代碼，使用預設值或從 BANK_NO 複製
        # 根據 D18，RED_BANK_NO 應該是銀行代碼 (007)，與 BANK_NO 相同
        red_bank = col(r, 'BANK_NO') or '007'  # 預設 007
        set_val('RED_BANK_NO', f"'{red_bank}'")
        
        # 金額 NTD 欄位
        min_init_ntd = col(r, 'MIN_INITIAL_PURCHASE_NTD') or col(r, 'MIN_INITIAL_PURCHASE') or '3000'
        min_amt_ntd = col(r, 'MIN_AMOUNT_NTD') or col(r, 'MIN_AMOUNT') or '3000'
        period_min_ntd = col(r, 'PERIOD_MIN_NTD') or '3000'
        min_red_amt_ntd = col(r, 'MIN_RED_AMOUNT_NTD') or '0'
        min_bal_amt_ntd = col(r, 'MIN_BAL_AMOUNT_NTD') or col(r, 'MIN_BAL_AMOUNT') or '0'
        ec_period_min_ntd = col(r, 'EC_PERIOD_MIN_NTD') or col(r, 'EC_PERIOD_MIN') or '3000'
        ec_min_amt_ntd = col(r, 'EC_MIN_AMOUNT_NTD') or col(r, 'EC_MIN_AMOUNT') or '3000'
        
        set_val('MIN_INITIAL_PURCHASE_NTD', min_init_ntd)
        set_val('MIN_AMOUNT_NTD', min_amt_ntd)
        set_val('PERIOD_MIN_NTD', period_min_ntd)
        set_val('MIN_RED_AMOUNT_NTD', min_red_amt_ntd)
        set_val('MIN_BAL_AMOUNT_NTD', min_bal_amt_ntd)
        set_val('EC_PERIOD_MIN_NTD', ec_period_min_ntd)
        set_val('EC_MIN_AMOUNT_NTD', ec_min_amt_ntd)
        
        # 日期欄位
        pur_cutoff = parse_date(col(r, 'PUR_CUT_OFF'))
        red_cutoff = parse_date(col(r, 'RED_CUT_OFF'))
        ec_pur_cutoff = parse_date(col(r, 'EC_PUR_CUT_OFF'))
        ec_red_cutoff = parse_date(col(r, 'EC_RED_CUT_OFF'))
        first_red = parse_date(col(r, 'FIRST_RED_DATE'))
        
        if pur_cutoff != 'null':
            set_val('PUR_CUT_OFF', pur_cutoff)
        if red_cutoff != 'null':
            set_val('RED_CUT_OFF', red_cutoff)
        if ec_pur_cutoff != 'null':
            set_val('EC_PUR_CUT_OFF', ec_pur_cutoff)
        if ec_red_cutoff != 'null':
            set_val('EC_RED_CUT_OFF', ec_red_cutoff)
        if first_red != 'null':
            set_val('FIRST_RED_DATE', first_red)

        line_val = ",".join(vals)
        new_lines.append(header_insert + line_val + ");")

    with open(SQL_PATH, 'w', encoding='utf-8') as f:
        f.write('\n'.join(new_lines))
        f.write('\n')

    print(f'Regenerated {len(rows)} INSERT rows to {SQL_PATH}')

if __name__ == '__main__':
    main()
