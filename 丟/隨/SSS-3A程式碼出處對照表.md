# SSS-3A 系統問題解答 - 程式碼出處對照表

本文件列出每個問題答案的程式碼出處，方便檢查邏輯是否正確。

---

## 一、查詢條件相關問題

### 3. 實際的查詢日期是看那一個？是期間變化日期區間還是期初申購日大於？

**答案邏輯出處：**

**期間變化日期區間（起日和訖日）：**
- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：49-65（新增契約金額）
- 程式碼：
```sql
and fas.purchase.ac_date between a.date_from and a.date_to
```
- 說明：`date_from` 和 `date_to` 來自查詢參數，用於篩選期間內發生的交易

**期初申購日大於（init_date）：**
- 檔案：`丟/SSS/d_sss3a_query.srd`
- 行數：21
- 程式碼：
```
column=(type=datetime updatewhereclause=no name=init_date dbname="init_date" initial="2008/01/01" )
```
- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：111, 129, 147, 164, 182, 200, 218, 234, 249, 265, 276, 311, 329, 347, 366, 380, 396, 411, 428, 442, 458, 482
- 程式碼：
```sql
and fas.purchase.ac_date >= :ad_init_date
```
- 說明：所有查詢都會檢查 `ac_date >= init_date`，預設值為 2008/01/01

**存量指標的日期條件：**
- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：208-226（有效契約金額）
- 程式碼：
```sql
and fas.purchase.ac_date <= a.date_to
and fas.purchase.maturity_date > a.date_to
and fas.purchase.ac_date >= :ad_init_date
```

---

### 4. 依客戶、依契約差在那裡？

**答案邏輯出處：**

**依客戶（amount_type = 'B'）：**
- 檔案：`丟/SSS/w_sss3a.srw`
- 行數：584-600
- 程式碼：
```sql
INSERT INTO "FAS"."SSS5A_ID_LIST"  
         ( "ID", "AMOUNT", "EMP_NO", "CREATED_DATE" )  
SELECT	fas.purchase.id,
        Sum(fas.purchase.amount * fas.f_fund_currency_rate(fas.purchase.fund_no, fas.purchase.ac_date)),
        :gs_id,
        :idt_exec_time
FROM	fas.purchase,
        ( select fund_no, decode(is_tdcc,'Y',:ld_date_to1,:ld_date_to) date_to, 
                  decode(is_tdcc,'Y',:ld_date_from1,:ld_date_from) date_from from fas.fund ) a
WHERE	fas.purchase.fund_no = a.fund_no
    and	fas.purchase.product_type = 'L2'
	and	fas.purchase.ac_date >= :ld_init_date
	and	fas.purchase.ac_date <= a.date_to
    and	fas.purchase.maturity_date > a.date_to
group by	fas.purchase.id;
```
- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：60-64, 77-81, 94-98, 112-116, 130-134, 148-152, 166-170, 184-188, 202-206, 220-224, 238-242, 296-299
- 程式碼：
```sql
and ( :as_amount_type <> 'B' or ( fas.purchase.id in ( 
    SELECT id FROM fas.sss5a_id_list 
    WHERE emp_no = :as_emp_no 
    and amount between :adec_min_amount and :adec_max_amount 
)))
```
- 說明：先計算每個客戶的總金額存入臨時表，再篩選符合範圍的客戶

**依契約（amount_type = 'C'）：**
- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：65, 82, 99, 117, 135, 153, 171, 189, 207, 225, 239, 273, 300
- 程式碼：
```sql
and ( :as_amount_type <> 'C' or ( 
    fas.purchase.amount * fas.f_fund_currency_rate(fas.purchase.fund_no, fas.purchase.ac_date) 
    between :adec_min_amount and :adec_max_amount 
))
```
- 說明：直接篩選每個契約的金額是否符合範圍

---

## 二、明細相關問題

### 2. 契約淨額是指什麼？

**答案邏輯出處：**

**新增契約金額（new_amount）：**
- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：49-65
- 程式碼：
```sql
SELECT	Sum(fas.purchase.amount * fas.f_fund_currency_rate(fas.purchase.fund_no, fas.purchase.ac_date)) new_amount
FROM	fas.purchase,
        sss.team_member,
        ( select fund_no, decode(is_tdcc,'Y',:ad_date_to1,:ad_date_to) date_to, 
                  decode(is_tdcc,'Y',:ad_date_from1,:ad_date_from) date_from, fund_group, sname from fas.fund ) a
WHERE	fas.purchase.sales_no = smart_stat.sales_no
    and	fas.purchase.product_type = 'L2'
	and	fas.purchase.ac_date between a.date_from and a.date_to
```
- 說明：統計期間內新增的 L2 契約金額

**到期契約金額（due_amount）：**
- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：100-117
- 程式碼：
```sql
SELECT	Sum(fas.purchase.amount * fas.f_fund_currency_rate(fas.purchase.fund_no, fas.purchase.ac_date)) due_amount
FROM	fas.purchase,
        sss.team_member,
        ( select fund_no, decode(is_tdcc,'Y',:ad_date_to1,:ad_date_to) date_to, 
                  decode(is_tdcc,'Y',:ad_date_from1,:ad_date_from) date_from, fund_group, sname from fas.fund ) a
WHERE	fas.purchase.sales_no = smart_stat.sales_no
    and	fas.purchase.product_type = 'L2'
	and	fas.purchase.maturity_date between a.date_from and a.date_to
	and	fas.purchase.ac_date >= :ad_init_date
```
- 說明：統計期間內到期的 L2 契約金額

**契約淨額（net_amount）：**
- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：590
- 程式碼：
```
compute(band=detail alignment="1" expression="new_amount - due_amount"
```
- 說明：計算欄位，直接相減

---

### 3. 停利金額是指已達停利該子基金的買回總金額是嗎？

**答案邏輯出處：**

- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：172-189
- 程式碼：
```sql
SELECT	Sum( fas.purchase.amount * fas.f_fund_currency_rate(fas.purchase.fund_no, fas.purchase.ac_date) ) profit
FROM	fas.purchase,
        sss.team_member,
        ( select fund_no, decode(is_tdcc,'Y',:ad_date_to1,:ad_date_to) date_to, 
                  decode(is_tdcc,'Y',:ad_date_from1,:ad_date_from) date_from, fund_group, sname from fas.fund ) a
WHERE	fas.purchase.sales_no = smart_stat.sales_no
    and	fas.purchase.product_type = 'L4'
	and	fas.purchase.ac_date >= :ad_init_date
    and fas.purchase.ac_date between a.date_from and a.date_to
```
- 說明：使用 `product_type = 'L4'` 識別停利交易，統計期間內的停利金額

---

### 4. 扣款金額和累計扣款金額差在那裡？

**答案邏輯出處：**

**扣款金額（investment）：**
- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：154-171
- 程式碼：
```sql
SELECT	Sum( fas.purchase.amount * fas.f_fund_currency_rate(fas.purchase.fund_no, fas.purchase.ac_date) ) investment
FROM	fas.purchase,
        sss.team_member,
        ( select fund_no, decode(is_tdcc,'Y',:ad_date_to1,:ad_date_to) date_to, 
                  decode(is_tdcc,'Y',:ad_date_from1,:ad_date_from) date_from, fund_group, sname from fas.fund ) a
WHERE	fas.purchase.sales_no = smart_stat.sales_no
    and	fas.purchase.product_type = 'L3'
	and	fas.purchase.ac_date >= :ad_init_date
    and fas.purchase.ac_date between a.date_from and a.date_to
```
- 說明：使用 `product_type = 'L3'`，日期條件是 `ac_date between date_from and date_to`（期間變化）

**累計扣款金額（active_investment）：**
- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：227-241
- 程式碼：
```sql
SELECT	Sum( fas.purchase.amount * fas.f_fund_currency_rate(fas.purchase.fund_no, fas.purchase.ac_date) ) active_investment
FROM	fas.purchase,
        sss.team_member,
        ( select fund_no, decode(is_tdcc,'Y',:ad_date_to1,:ad_date_to) date_to, 
                  decode(is_tdcc,'Y',:ad_date_from1,:ad_date_from) date_from, fund_group, sname from fas.fund ) a
WHERE	fas.purchase.sales_no = smart_stat.sales_no
    and	fas.purchase.product_type = 'L3'
	and	fas.purchase.ac_date >= :ad_init_date
    and fas.purchase.ac_date <= a.date_to
	and	fas.purchase.maturity_date > a.date_to
```
- 說明：使用 `product_type = 'L3'`，日期條件是 `ac_date <= date_to` 且 `maturity_date > date_to`（有效契約存量）

---

### 5. 有效契約金額是指此母基金截至輸入的迄日的市值？成本？

**答案邏輯出處：**

- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：208-226
- 程式碼：
```sql
SELECT	sum(fas.purchase.amount * fas.f_fund_currency_rate(fas.purchase.fund_no, fas.purchase.ac_date)) active_amount
FROM	fas.purchase,
        sss.team_member,
        ( select fund_no, decode(is_tdcc,'Y',:ad_date_to1,:ad_date_to) date_to, 
                  decode(is_tdcc,'Y',:ad_date_from1,:ad_date_from) date_from, fund_group, subta_no, sname from fas.fund ) a
WHERE	fas.purchase.sales_no = smart_stat.sales_no
    and	fas.purchase.product_type = 'L2'
	and	fas.purchase.ac_date >= :ad_init_date
    and fas.purchase.ac_date <= a.date_to
	and	fas.purchase.maturity_date > a.date_to
```
- 說明：使用 `fas.purchase.amount`（原始申購金額）乘以匯率，不是淨值，所以是成本

---

### 6. 截止日餘額是指母基金嗎？

**答案邏輯出處：**

- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：274-300
- 程式碼：
```sql
SELECT	sum(fas.certificate.shares * n.nav * fas.f_fund_currency_rate(pur.fund_no, a.date_to)) fum
from	fas.certificate,
        ( select fas.purchase.*, amount*fas.f_fund_currency_rate(fas.purchase.fund_no, fas.purchase.ac_date) amount_ntd 
          from fas.purchase, fas.fund 
          where ( fas.purchase.fund_no=fas.fund.fund_no) 
            and product_type='L2' 
            and fas.purchase.ac_date >= :ad_init_date) pur,
        ( select fas.purchase.id, fas.purchase.fund_no, fas.purchase.master_no 
          from fas.purchase, fas.fund 
          where ( fas.purchase.fund_no=fas.fund.fund_no) 
            and ( product_type='L2' or product_type='L3' or product_type='L4') 
            and fas.purchase.ac_date >= :ad_init_date 
          group by fas.purchase.id, fas.purchase.fund_no, fas.purchase.master_no) pur1,
        ( select fund_no, nav_date, nav from fas.nav 
          where nav_date = :ad_date_to1 or nav_date = :ad_date_to ) n,
        ( select fund_no, decode(is_tdcc,'Y',:ad_date_to1,:ad_date_to) date_to, 
                  decode(is_tdcc,'Y',:ad_date_from1,:ad_date_from) date_from, fund_group, sname from fas.fund ) a
where	pur.sales_no = smart_stat.sales_no
    and pur.fund_no = smart_stat.fund_no
    and pur1.master_no = pur.master_no
    and pur.maturity_date > a.date_to
    and pur.ac_date <= a.date_to
    and a.fund_no = pur1.fund_no
    and fas.certificate.id = pur1.id
    and fas.certificate.fund_no = pur1.fund_no
    and fas.certificate.master_no = pur1.master_no
    and fas.certificate.master_no > 0
    and fas.certificate.issue_date <= a.date_to
    and (	fas.certificate.cancel_date is null
        or	fas.certificate.cancel_date > a.date_to )
    and fas.certificate.fund_no = a.fund_no
    and n.fund_no = a.fund_no
    and n.nav_date = a.date_to
```
- 說明：
  - 計算公式：`certificate.shares × nav.nav × f_fund_currency_rate(fund_no, date_to)`
  - 使用 `product_type='L2'` 的 purchase 資料（效率投資 L2 契約）
  - 使用 `date_to` 當日的淨值（`nav_date = a.date_to`）
  - 只統計有效的持份證明（`issue_date <= date_to` 且 `cancel_date is null` 或 `cancel_date > date_to`）

**注意：** 程式碼中使用的是 `product_type='L2'` 來識別效率投資契約，並沒有直接使用 `fas.fund.fund_master_no` 欄位來判斷「母基金」。

---

### 7. 全部契約金額是指有效＋無效？

**答案邏輯出處：**

- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：301-318
- 程式碼：
```sql
SELECT	Sum(fas.purchase.amount * fas.f_fund_currency_rate(fas.purchase.fund_no, fas.purchase.ac_date)) total_amount
from	fas.purchase,
        sss.team_member,
        ( select fund_no, decode(is_tdcc,'Y',:ad_date_to1,:ad_date_to) date_to, 
                  decode(is_tdcc,'Y',:ad_date_from1,:ad_date_from) date_from, fund_group, subta_no, sname from fas.fund ) a
where	fas.purchase.sales_no = smart_stat.sales_no
    and	fas.purchase.product_type = 'L2'
	and	fas.purchase.ac_date >= :ad_init_date
	and	fas.purchase.ac_date <= a.date_to
```
- 說明：只有 `ac_date <= date_to` 條件，沒有 `maturity_date` 的限制，所以包括有效和無效的契約

---

### 8. 手續費在停利金額右側的是指子轉母手續費是嗎？而全部契約的手續費是指母轉子？

**答案邏輯出處：**

**停利金額右側的手續費（sales_charge）：**
- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：190-207
- 程式碼：
```sql
SELECT	Sum( fas.purchase.service_charge * fas.f_fund_currency_rate(fas.purchase.fund_no, fas.purchase.ac_date) ) sales_charge
FROM	fas.purchase,
        sss.team_member,
        ( select fund_no, decode(is_tdcc,'Y',:ad_date_to1,:ad_date_to) date_to, 
                  decode(is_tdcc,'Y',:ad_date_from1,:ad_date_from) date_from, fund_group, sname from fas.fund ) a
WHERE	fas.purchase.sales_no = smart_stat.sales_no
    and	( fas.purchase.product_type = 'L2' or fas.purchase.product_type = 'L3' or fas.purchase.product_type = 'L4')
	and	fas.purchase.ac_date >= :ad_init_date
    and fas.purchase.ac_date between a.date_from and a.date_to
```
- 說明：統計 L2、L3、L4 的手續費，日期條件是 `ac_date between date_from and date_to`（期間變化）

**全部契約的手續費（total_sales_charge）：**
- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：355-372
- 程式碼：
```sql
SELECT	Sum(fas.purchase.service_charge * fas.f_fund_currency_rate(fas.purchase.fund_no, fas.purchase.ac_date)) total_sales_charge
FROM	fas.purchase,
        sss.team_member,
        ( select fund_no, decode(is_tdcc,'Y',:ad_date_to1,:ad_date_to) date_to, 
                  decode(is_tdcc,'Y',:ad_date_from1,:ad_date_from) date_from, fund_group, sname from fas.fund ) a
WHERE	fas.purchase.sales_no = smart_stat.sales_no
    and	( fas.purchase.product_type = 'L2' or fas.purchase.product_type = 'L3' or fas.purchase.product_type = 'L4')
	and	fas.purchase.ac_date <= a.date_to
	and	fas.purchase.ac_date >= :ad_init_date
```
- 說明：統計 L2、L3、L4 的手續費，日期條件是 `ac_date <= date_to`（截至 date_to）

---

### 9. 以上的金額在換算為台幣時，皆是按契日的匯率進行換算嗎？

**答案邏輯出處：**

- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：49, 65, 100, 117, 154, 171, 172, 189, 190, 207, 208, 226, 227, 241, 274, 300, 301, 318, 355, 372（所有金額計算）
- 程式碼：
```sql
fas.purchase.amount * fas.f_fund_currency_rate(fas.purchase.fund_no, fas.purchase.ac_date)
```
- 說明：所有金額計算都使用 `f_fund_currency_rate(fund_no, ac_date)`，第二個參數是 `ac_date`（契約日），所以是按契約日的匯率換算

**例外：截止日餘額（FUM）**
- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：274
- 程式碼：
```sql
fas.f_fund_currency_rate(pur.fund_no, a.date_to)
```
- 說明：FUM 使用 `date_to` 的匯率，因為是計算截至 `date_to` 的市值

---

### 10. 下方契約內的類別有那些？另新戶怎麼定義？

**答案邏輯出處：**

**類別（cancel_reason）：**
- 檔案：`丟/SSS/d_sss3a_contract.srd`
- 行數：127
- 程式碼：
```
compute(band=header.1 alignment="2" expression="case( cancel_reason when 'E' then '提前解約' when 'L' then '餘額不足' when 'M' then '契約到期' else if( prev_amount >= adec_min_amount and prev_amount <= adec_max_amount, '', '新戶' ))"
```
- 說明：
  - 'E' = 提前解約
  - 'L' = 餘額不足
  - 'M' = 契約到期
  - 空值 = 根據 `prev_amount` 判斷是否為新戶

**期初金額（prev_amount）：**
- 檔案：`丟/SSS/d_sss3a_contract.srd`
- 行數：53-67
- 程式碼：
```sql
( SELECT	Sum(fas.purchase.amount * fas.v_currency_rate.ex_rate)
	FROM	fas.purchase,
			fas.fund,
			fas.v_currency_rate,
			( select fund_no, decode(is_tdcc,'Y',:ad_date_to1,:ad_date_to) date_to, 
                      decode(is_tdcc,'Y',:ad_date_from1,:ad_date_from) date_from from fas.fund ) a
WHERE	fas.purchase.id = fas.holder.id
	and	fas.purchase.fund_no = fas.fund.fund_no
	and fas.fund.fund_no = a.fund_no
	and	fas.v_currency_rate.fund_no = fas.fund.fund_no
	and	fas.v_currency_rate.tx_date = fas.purchase.ac_date
	and	fas.purchase.product_type = 'L2'
	and	fas.purchase.ac_date >= :ad_init_date
	and	fas.purchase.ac_date <= (a.date_from - 1)
	and	fas.purchase.maturity_date > (a.date_from - 1)
) prev_amount,
```
- 說明：統計該客戶在 `date_from - 1` 之前的所有有效 L2 契約金額總和

**新戶定義：**
- 檔案：`丟/SSS/d_sss3a_contract.srd`
- 行數：127
- 程式碼：
```
if( prev_amount >= adec_min_amount and prev_amount <= adec_max_amount, '', '新戶' )
```
- 說明：如果 `prev_amount` 不在 `min_amount` 和 `max_amount` 之間，則顯示「新戶」

---

### 11. 金額級距指什麼

**答案邏輯出處：**

- 檔案：`丟/SSS/d_sss3a_contract.srd`
- 行數：68
- 程式碼：
```sql
fas.purchase.amount amount_level /* DUMMY for dynamic inquiry */
```
- 檔案：`丟/SSS/d_sss3a_contract.srd`
- 行數：31
- 程式碼：
```
column=(type=decimal(2) updatewhereclause=yes name=amount_level dbname="amount_level" )
```
- 說明：虛擬欄位，實際值是 `fas.purchase.amount`（原幣金額），用於動態查詢

---

## 補充說明

### init_date 預設值
- 檔案：`丟/SSS/d_sss3a_query.srd`
- 行數：21
- 程式碼：
```
initial="2008/01/01"
```

### 依客戶的臨時表建立
- 檔案：`丟/SSS/w_sss3a.srw`
- 行數：584-600（SSS5A_ID_LIST）
- 行數：609-625（SSS5A_MASTER_LIST）
- 說明：在查詢前先建立臨時表，計算每個客戶和每個母基金契約的總金額

### MA 基金的日期調整
- 檔案：`丟/SSS/w_sss3a.srw`
- 行數：515-525
- 程式碼：
```
if ls_ma_type='1' then   // 需求為扣除4個營業日
	ld_date_to1=f_relative_date_tdcc(ld_date_to,-4)
else
	ld_date_to1=ld_date_to
end if

if ls_ma_type='1' then   // 需求為扣除4個營業日
	ld_date_from1=f_relative_date_tdcc(ld_date_from,-4)
else
	ld_date_from1=ld_date_from
end if
```
- 說明：如果選擇「Non MA」，會將日期往前推 4 個營業日

### 日期調整在查詢中的使用
- 檔案：`丟/SSS/d_sss3a.srd`
- 行數：52, 69, 86, 103, 120, 137, 157, 175, 193, 210, 228, 279
- 程式碼：
```sql
( select fund_no, decode(is_tdcc,'Y',:ad_date_to1,:ad_date_to) date_to, 
          decode(is_tdcc,'Y',:ad_date_from1,:ad_date_from) date_from, fund_group, sname from fas.fund ) a
```
- 說明：如果基金是 TDCC（`is_tdcc='Y'`），使用調整後的日期（`date_to1`, `date_from1`），否則使用原始日期

---

## 檢查重點

1. **所有金額計算都使用 `f_fund_currency_rate(fund_no, ac_date)`**，除了 FUM 使用 `date_to` 的匯率
2. **所有查詢都會檢查 `ac_date >= init_date`**
3. **期間變化指標使用 `ac_date between date_from and date_to`**
4. **存量指標使用 `ac_date <= date_to` 且 `maturity_date > date_to`**
5. **依客戶使用臨時表 `fas.sss5a_id_list`**
6. **依契約直接篩選 `purchase.amount`**
7. **截止日餘額（FUM）使用 `certificate.shares × nav.nav × 匯率`**，且只統計 `product_type='L2'` 的契約

