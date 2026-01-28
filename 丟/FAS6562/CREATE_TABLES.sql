-- ============================================
-- 建立 FILE 06 & FILE 07 所需的資料表
-- ============================================

-- ============================================
-- TABLE 1: fas.new_agent_6562_temp1
-- 用途：Letter of Declaration with ISIN 月報資料
-- ============================================
CREATE TABLE fas.new_agent_6562_temp1 (
    fund_no                VARCHAR2(10) NOT NULL,           -- 基金代號
    tx_date                DATE NOT NULL,                   -- 交易日期
    isin_code              VARCHAR2(20),                    -- ISIN代碼
    fund_name              VARCHAR2(200),                   -- 基金名稱
    fund_size              NUMBER(20,2),                    -- 基金規模 (USD)
    derivative_enhance     NUMBER(20,2),                    -- 衍生工具部位-強化績效
    limit_40_pct           NUMBER(10,4),                    -- 40%限制比率
    derivative_hedge       NUMBER(20,2),                    -- 衍生工具部位-避險目的
    denominator            NUMBER(20,2),                    -- 分母(相關證券市值)
    limit_100_pct          NUMBER(10,4),                    -- 100%限制比率
    invest_taiwan          NUMBER(20,2),                    -- 台灣投資金額
    limit_50_pct           NUMBER(10,4),                    -- 50%限制比率
    invest_china           NUMBER(20,2),                    -- 中國證券投資金額
    limit_20_pct           NUMBER(10,4),                    -- 20%限制比率
    create_date            DATE DEFAULT SYSDATE,            -- 建立日期
    update_date            DATE DEFAULT SYSDATE,            -- 更新日期
    CONSTRAINT pk_new_agent_6562_temp1 PRIMARY KEY (fund_no, tx_date)
);

-- 建立索引
CREATE INDEX idx_temp1_isin ON fas.new_agent_6562_temp1(isin_code);
CREATE INDEX idx_temp1_tx_date ON fas.new_agent_6562_temp1(tx_date);

-- 加入註解
COMMENT ON TABLE fas.new_agent_6562_temp1 IS 'Letter of Declaration with ISIN 月報資料';
COMMENT ON COLUMN fas.new_agent_6562_temp1.fund_no IS '基金代號';
COMMENT ON COLUMN fas.new_agent_6562_temp1.tx_date IS '交易日期';
COMMENT ON COLUMN fas.new_agent_6562_temp1.isin_code IS 'ISIN代碼';
COMMENT ON COLUMN fas.new_agent_6562_temp1.fund_name IS '基金名稱';
COMMENT ON COLUMN fas.new_agent_6562_temp1.fund_size IS '基金規模(USD)';
COMMENT ON COLUMN fas.new_agent_6562_temp1.derivative_enhance IS '衍生工具部位-強化績效';
COMMENT ON COLUMN fas.new_agent_6562_temp1.limit_40_pct IS '40%限制比率';
COMMENT ON COLUMN fas.new_agent_6562_temp1.derivative_hedge IS '衍生工具部位-避險目的';
COMMENT ON COLUMN fas.new_agent_6562_temp1.denominator IS '分母(相關證券市值)';
COMMENT ON COLUMN fas.new_agent_6562_temp1.limit_100_pct IS '100%限制比率';
COMMENT ON COLUMN fas.new_agent_6562_temp1.invest_taiwan IS '台灣投資金額';
COMMENT ON COLUMN fas.new_agent_6562_temp1.limit_50_pct IS '50%限制比率';
COMMENT ON COLUMN fas.new_agent_6562_temp1.invest_china IS '中國證券投資金額';
COMMENT ON COLUMN fas.new_agent_6562_temp1.limit_20_pct IS '20%限制比率';

-- ============================================
-- TABLE 2: fas.new_agent_6562_temp2
-- 用途：SITCA AUM 月報資料
-- ============================================
CREATE TABLE fas.new_agent_6562_temp2 (
    fund_no                     VARCHAR2(10) NOT NULL,      -- 基金代號
    tx_date                     DATE NOT NULL,              -- 交易日期
    fund_abbr                   VARCHAR2(50),               -- 基金簡稱
    seq_no                      NUMBER(10),                 -- 序號
    fund_name                   VARCHAR2(200),              -- 基金名稱
    isin_code                   VARCHAR2(20),               -- ISIN代碼
    fbc                         VARCHAR2(10),               -- 基金幣別
    fund_size_product_ccy       NUMBER(20,2),               -- 基金規模(產品幣別)
    fx_rate                     NUMBER(10,5),               -- 匯率
    nav_reporting_ccy           NUMBER(20,4),               -- NAV(報告幣別)
    invest_amt_taiwan_usd       NUMBER(20,2),               -- 台灣投資人投資金額(USD)
    fund_size_by_share_class    NUMBER(20,2),               -- 分級別基金規模(USD)
    fund_size_global_aa         NUMBER(20,2),               -- 全球基金規模AA(USD)
    invest_pct                  NUMBER(10,4),               -- 台灣投資人占比
    create_date                 DATE DEFAULT SYSDATE,       -- 建立日期
    update_date                 DATE DEFAULT SYSDATE,       -- 更新日期
    CONSTRAINT pk_new_agent_6562_temp2 PRIMARY KEY (fund_no, tx_date)
);

-- 建立索引
CREATE INDEX idx_temp2_isin ON fas.new_agent_6562_temp2(isin_code);
CREATE INDEX idx_temp2_tx_date ON fas.new_agent_6562_temp2(tx_date);
CREATE INDEX idx_temp2_fund_abbr ON fas.new_agent_6562_temp2(fund_abbr);

-- 加入註解
COMMENT ON TABLE fas.new_agent_6562_temp2 IS 'SITCA AUM 月報資料';
COMMENT ON COLUMN fas.new_agent_6562_temp2.fund_no IS '基金代號';
COMMENT ON COLUMN fas.new_agent_6562_temp2.tx_date IS '交易日期';
COMMENT ON COLUMN fas.new_agent_6562_temp2.fund_abbr IS '基金簡稱';
COMMENT ON COLUMN fas.new_agent_6562_temp2.seq_no IS '序號';
COMMENT ON COLUMN fas.new_agent_6562_temp2.fund_name IS '基金名稱';
COMMENT ON COLUMN fas.new_agent_6562_temp2.isin_code IS 'ISIN代碼';
COMMENT ON COLUMN fas.new_agent_6562_temp2.fbc IS '基金幣別';
COMMENT ON COLUMN fas.new_agent_6562_temp2.fund_size_product_ccy IS '基金規模(產品幣別)';
COMMENT ON COLUMN fas.new_agent_6562_temp2.fx_rate IS '匯率';
COMMENT ON COLUMN fas.new_agent_6562_temp2.nav_reporting_ccy IS 'NAV(報告幣別)';
COMMENT ON COLUMN fas.new_agent_6562_temp2.invest_amt_taiwan_usd IS '台灣投資人投資金額(USD)';
COMMENT ON COLUMN fas.new_agent_6562_temp2.fund_size_by_share_class IS '分級別基金規模(USD)';
COMMENT ON COLUMN fas.new_agent_6562_temp2.fund_size_global_aa IS '全球基金規模AA(USD)';
COMMENT ON COLUMN fas.new_agent_6562_temp2.invest_pct IS '台灣投資人占比';

-- ============================================
-- 查詢資料範例
-- ============================================
/*
-- 查詢 temp1 資料
SELECT *
  FROM fas.new_agent_6562_temp1
 WHERE tx_date = TO_DATE('2025-10-31', 'YYYY-MM-DD')
 ORDER BY fund_no;

-- 查詢 temp2 資料
SELECT *
  FROM fas.new_agent_6562_temp2
 WHERE tx_date = TO_DATE('2025-10-31', 'YYYY-MM-DD')
 ORDER BY seq_no;

-- 刪除測試資料
DELETE FROM fas.new_agent_6562_temp1 WHERE tx_date = TO_DATE('2025-10-31', 'YYYY-MM-DD');
DELETE FROM fas.new_agent_6562_temp2 WHERE tx_date = TO_DATE('2025-10-31', 'YYYY-MM-DD');
COMMIT;
*/

-- ============================================
-- 授權語法 (如需要)
-- ============================================
/*
GRANT SELECT, INSERT, UPDATE, DELETE ON fas.new_agent_6562_temp1 TO your_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON fas.new_agent_6562_temp2 TO your_user;
*/
