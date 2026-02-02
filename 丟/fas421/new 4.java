string ls_stage , ls_flag , ls_status , ls_fund_no , ls_year , ls_dividend_type , ls_org_currency_no , ls_id , ls_pay_type
string ls_fund_no_in , ls_pay_address , ls_fund_currency_no , ls_fund_category , ls_upd_bank_no , ls_upd_branch_no 
string ls_upd_ac_code , ls_upd_fis_sname , ls_upd_currency_no , ls_upd_pay_address , ls_upd_swift_code , old_fund_no
string ls_upd_ac_name , ls_upd_intermediary_bank_no , ls_upd_intermediary_ac_code , ls_broker_currency_no
string ls_fund_master , ls_fund_master_in , ls_holder_status , ls_sql, ls_amc_no, ls_bel_cus_no , ls_offering_type
long ll_count , i , j , ll_row , ll_count1 , ll_count2 , ll_count_f ,ll_count_h, ll_id_seq , k , ii , f
date ld_dividend_date , ld_payment_date , ld_next_dividend_date , ld_pay_date , ld_evaluated_date , ld_really_div_date , ld_ac_date
dec  ldec_dividend_rate , ldec_stock_rate , ldec_interest_rate , ldec_security_rate , ldec_shares , ldec_dividend_min
dec  ldec_amt_decimal , ldec_wire_fee , ldec_total_return , ldec_amount , ldec_stock_amount , ldec_security_amount , ldec_security_rate_onshore
dec  ldec_interest_amount , ldec_sum_shares , ldec_sum_amount , ldec_security_amount_onshore,ldec_year_rate,ldec_capital_rate,ldec_interest_rate_f ,ldec_difference
dec  ldec_cer_shares
//SR01658472 配息來自於本金應不可計入海外所得 Modified by Eric
dec  ldec_capital_amount
decimal ldec_dividend_max , dividend_array[] , ldec_dividend_temp
datetime ldt_prog_ctrl_date

setnull(ldt_prog_ctrl_date)

SELECT "RIS"."PROG_CTRL"."PROG_CTRL_DATE"  
    INTO :ldt_prog_ctrl_date  
    FROM "RIS"."PROG_CTRL"  
   WHERE ( RIS."PROG_CTRL"."AP_CODE" = 'FAS' ) AND  
	          ( RIS."PROG_CTRL"."MENU_ID" = '421' ) AND  
   	          ( RIS."PROG_CTRL"."PROG_CTRL_CODE" = 'ROUND_TYPE' ) AND  
         	 ( RIS."PROG_CTRL"."FLAG" = 'Y' )   ;
			
f_sql_code()	

dw_2.AcceptText()
SetPointer(HourGlass!)
cb_1_new.Enabled          = false 
ls_stage              = '產生境外基金收益分配資料'
ll_count = 0

MessageBox('注意','請重新執行FAS422、FAS423')

for i = 1 to dw_2.RowCount()
	 ls_flag   			= dw_2.GetItemString(i,'flag')
	 ls_status 		= dw_2.GetItemString(i,'status')
	 ls_fund_no      = dw_2.GetItemString(i,'fund_no')
//	k = i
//	f= dw_2.RowCount()
	 /* 2015.9.18 by Anita
	 產生資料時，請先檢查該基金記帳日是否大於基準日。
	 若記帳日<=基準日時，請出警語告知並不開放產生資料。
	 */
	 ld_dividend_date 	= Date(dw_2.GetItemDateTime(i,'dividend_date'))
	 	
	 select ac_date
		into :ld_ac_date
		 from fas.fund
		 where fund_no = :ls_fund_no ;
	
	 f_sql_code()
	 
	  ldec_cer_shares = 0
		 select sum(shares)
		  	into :ldec_cer_shares
			  from fas.certificate
			  where fund_no = :ls_fund_no
			  	and issue_date <= :ld_dividend_date
				and (cancel_date > :ld_dividend_date or cancel_date is null ) ;
		f_sql_code()
	
	 if ls_flag = 'Y' and ld_ac_date <= ld_dividend_date and ldec_cer_shares > 0 then
	 	 f_error(ls_fund_no + '-此基金記帳日<=收益分配基準日，無法重新產生資料')
		 cb_1_new.Enabled = true 
		 return 
	 end if
	 
	 if ls_status = 'C' and ls_flag = 'Y' then
		 f_error(ls_fund_no+'-此基金資料已確認，無法重新產生資料')
		 dw_2.SetItem(i,'flag','N')
		 ls_flag='N'
		 Continue
	 end if 
	 
	 if ls_flag = 'Y' and (ls_status <> 'C' or isnull(ls_status)) then
		 ls_fund_no            			= dw_2.GetItemString(i,'fund_no')
		 ld_dividend_date      			= Date(dw_2.GetItemDateTime(i,'dividend_date'))
		 ldec_dividend_rate    		= dw_2.GetItemDecimal(i,'dividend_rate')
		 ld_payment_date      		= Date(dw_2.GetItemDateTime(i,'payment_date'))
		 ld_next_dividend_date		= Date(dw_2.GetItemDateTime(i,'next_dividend_date'))
		 ls_year							= string(ld_dividend_date,'YYYY')
		 ld_pay_date					= Date(dw_2.GetItemDateTime(i,'pay_date'))
		 ld_evaluated_date     		= Date(dw_2.GetItemDateTime(i,'evaluated_date'))
		 ldec_stock_rate       			= dw_2.GetItemDecimal(i,'stock_rate')
		 ldec_interest_rate    			= dw_2.GetItemDecimal(i,'interest_rate')
		 ldec_security_rate    			= dw_2.GetItemDecimal(i,'security_rate')
		 ldec_security_rate_onshore = dw_2.GetItemDecimal(i,'security_rate_onshore')
		 ls_dividend_type      			 = dw_2.GetItemString(i,'dividend_type')
		 ld_really_div_date      		 = Date(dw_2.GetItemDateTime(i,'really_div_date'))		
		 ls_fund_category 				 = dw_2.GetItemString(i,'fund_category')
		 ls_amc_no						= dw_2.GetItemString(i,'amc_no')
		 ldec_year_rate                   =dw_2.GetItemDecimal(i,'year_rate')  //2015.12.01  新增經理公司為01時轉入檔案格式  (Clamp Lin)  需求單：001868
		 ldec_capital_rate                =dw_2.GetItemDecimal(i,'capital_rate')  //2015.12.01  新增經理公司為01時轉入檔案格式  (Clamp Lin)  需求單：001868
		 ldec_interest_rate_f            =dw_2.GetItemDecimal(i,'interest_rate_f')  //2015.12.01  新增經理公司為01時轉入檔案格式  (Clamp Lin)  需求單：001868
		 ls_offering_type 				 =dw_2.GetItemString(i,'offering_type') // NO001939 境內私募基金(Venus)
		 
		 dividend_array[] = {ldec_stock_rate, ldec_interest_rate_f, ldec_security_rate, ldec_security_rate_onshore, ldec_capital_rate} //(Kent) 找分配息率最大值
		 ldec_dividend_max = ldec_stock_rate //最大值預設
		 //  原本為 i ，會造成無窮迴圈 ，故改成ii Kent
		for ii = 1 to UpperBound (dividend_array[])
			for j = ii+1 to UpperBound (dividend_array[])
				if dividend_array[ii] < dividend_array[j] then
					ldec_dividend_temp = dividend_array[j]
				end if 
			next
		   if ldec_dividend_temp > ldec_dividend_max then
			  ldec_dividend_max = ldec_dividend_temp
		   end if 					
		 next
		 
		 select ac_date into :ld_ac_date
			from fas.fund
		  where fund_no = :ls_fund_no;
		
		 if f_sql_code() < 0 then
			 f_error(ls_stage + "(1)")
			 cb_1_new.Enabled = true 
			 return 
		 end if
		
		 if isnull(ld_dividend_date) then
			 f_error('收益分配基準日不可為空')
			 cb_1_new.Enabled = true 
			 return
		 end if 
		 
		if ld_ac_date <= ld_dividend_date and ldec_cer_shares > 0  then 
			f_error('配息基金結帳日 '+string(ld_ac_date,'YYYY/MM/DD')+ ' 不可大於收益分配基準日')
			cb_1_new.Enabled = true 
			return
		end if 		 
		
		if ld_ac_date > ld_payment_date and ldec_cer_shares > 0 then 
			f_error('配息基金結帳日 '+string(ld_ac_date,'YYYY/MM/DD')+ ' 不可大於收益分配發放日')
			cb_1_new.Enabled = true 
			return
		end if 
		
		 if isnull(ld_payment_date) then
			 f_error('收益分配發放日不可為空')
			 cb_1_new.Enabled = true 
			 return
		 end if 
		 
		 if isnull(ld_really_div_date) then
			 f_error('收益發放月份不可為空')
			 cb_1_new.Enabled = true 
			 return
		 end if 		 
		 //2014.03.27 檢核除息日、付款日、配息率不能為空值  (Clamp Lin)  需求單：001193
		 if isnull(ld_next_dividend_date) then
			 f_error('除息日不可為空')
			 cb_1_new.Enabled = true 
			 return
		 end if
		 
		 if isnull(ld_pay_date) then
			 f_error('付款日不可為空')
			 cb_1_new.Enabled = true 
			 return
		 end if 
		 
		 if isnull(ldec_dividend_rate) then
			 f_error('配息率不可為空')
			 cb_1_new.Enabled = true 
			 return
		 end if
		 
//		 if ld_payment_date < date(f_sysdate()) and ld_pay_date < date(f_sysdate()) and ldec_cer_shares > 0 then
//			f_error('發放日與付款日小於系統日，請確認!')
//			 cb_1_new.Enabled = true 
//			 return
//		 end if
		 //結束新增
		 if ls_fund_category = '1' then
			if isnull(ldec_stock_rate) then ldec_stock_rate = 0
			if isnull(ldec_interest_rate) then ldec_interest_rate = 0
			if isnull(ldec_security_rate) then ldec_security_rate = 0
			if isnull(ldec_security_rate_onshore) then ldec_security_rate_onshore = 0
			
			if ldec_dividend_rate <> ldec_stock_rate + ldec_interest_rate + ldec_security_rate + ldec_security_rate_onshore then
				f_error('配息率必須等於股利收入+利息收入+資本利得(海外)+資本利得(國內)')
				cb_1_new.Enabled = true
				return
			end if 
		end if 
		 
		
		 Delete from fas.dividend
			where fund_no       = :ls_fund_no
			  and dividend_date = :ld_dividend_date
			  and dividend_type  = :ls_dividend_type;
		
		 if f_sql_code() < 0 then
			 f_error(ls_stage + "(2)")
			 cb_1_new.Enabled = true 
			 return 
		 end if
		
		 Delete from fas.dividend_detail
			where fund_no       = :ls_fund_no
			  and dividend_date = :ld_dividend_date
			  and dividend_type  = :ls_dividend_type;
		
		 if f_sql_code() < 0 then
			 f_error(ls_stage + "(3)")
			 cb_1_new.Enabled = true 
			 return 
		 end if
		 
		 dw_2.SetItem(i,'emp_no',gs_id)
		 
		 insert into fas.dividend
		        	(	fund_no            				, dividend_year          	, dividend_date       , pay_date             ,
					payment_date       			, next_dividend_date		, dividend_rate        , status               , 
					evaluated_date     				, stock_rate             		, interest_rate         , security_rate        , 
					emp_no             				, last_modified          		, dividend_type        , really_div_date ,
					security_rate_onshore,year_rate,capital_rate,interest_rate_f		 )
					 
		 values (	:ls_fund_no       				, :ls_year               		, :ld_dividend_date   , :ld_pay_date        ,
		         	:ld_payment_date  			, :ld_next_dividend_date , :ldec_dividend_rate , 'O'                 ,
					:ld_evaluated_date			, :ldec_stock_rate       	, :ldec_interest_rate  , :ldec_security_rate ,
					:gs_id             					, sysdate						, :ls_dividend_type    , :ld_really_div_date,
					:ldec_security_rate_onshore,:ldec_year_rate,:ldec_capital_rate,:ldec_interest_rate_f ) ;
					
		 if f_sql_code() < 0 then
			 f_error(ls_stage + "(4)")
			 cb_1_new.Enabled = true 
			 return 
		 end if
		 
		 //no 001423 fas.holder_dividend若由前台轉入，則覆核人/覆核日期/狀態自動帶入，但因前台程式暫時未能上線
		 // 故先在fas421 update 這三個欄位
		 
		 update fas.holder_dividend
		 	set reviewed_by = 'ECS' ,
			 	review_date = revision_date,
				 status = 'R'
			where status <> 'R' 
				and created_by = 'ECS'
				and revised_by = 'ECS';
		
		 DECLARE	cs_fas421 CURSOR FOR
		 select distinct fas.certificate.currency_no,
				  fas.certificate.id,
				  sum(fas.certificate.shares),
				  fas.holder_dividend_amc.pay_type,
				  fas.holder_dividend_amc.fund_no_in,
				  fas.holder_dividend_amc.pay_address,
				  fas.holder_dividend.status,
				  fas.fund.currency_no,
				  fas.fund.fund_category,
				  fas.fund.dividend_min,
				  fas.fund.amt_decimal,
				  fas.fund.fund_master_no,
				  fas.certificate.bel_cus_no
			from fas.certificate , 
				  fas.holder_dividend,
				  fas.holder_dividend_amc,
				  fas.fund
		  where fas.certificate.fund_no    = fas.fund.fund_no
		  	 and fas.holder_dividend.id = fas.holder_dividend_amc.id(+)
			 and fas.holder_dividend_amc.amc_no(+) = :ls_amc_no
			 and fas.certificate.issue_date <= :ld_dividend_date
			 and (fas.certificate.cancel_date > :ld_dividend_date or fas.certificate.cancel_date is null)
			 and fas.certificate.fund_no = :ls_fund_no
			 and fas.certificate.shares  > 0
			 and fas.certificate.id      = fas.holder_dividend.id(+)
			// and fas.fund.fund_no not in ('M65','M66','M67')
		 group by fas.certificate.currency_no,
					 fas.certificate.id,
					fas.holder_dividend_amc.pay_type,
				  	fas.holder_dividend_amc.fund_no_in,
				  	fas.holder_dividend_amc.pay_address,
					 fas.holder_dividend.status,
					 fas.fund.currency_no,
					 fas.fund.fund_category,
					 fas.fund.dividend_min,
					 fas.fund.amt_decimal,
					 fas.fund.fund_master_no,
				      fas.certificate.bel_cus_no
		 order by :ls_fund_no,
					 fas.certificate.id,
					 fas.certificate.currency_no,
				     fas.certificate.bel_cus_no ;
		
		 OPEN cs_fas421;
		
		 if f_sql_code() < 0 then
			 f_error(ls_stage + "(5)")
			 cb_1_new.Enabled = true 
			 CLOSE cs_fas421;
			 return 
		 end if
		
		 FETCH cs_fas421
		  INTO :ls_org_currency_no,
				 :ls_id,
				 :ldec_shares,
				 :ls_pay_type,
				 :ls_fund_no_in,
				 :ls_pay_address,
				 :ls_holder_status,
				 :ls_fund_currency_no,
				 :ls_fund_category,
				 :ldec_dividend_min,
				 :ldec_amt_decimal,
				 :ls_fund_master,
				 :ls_bel_cus_no;
		 
		 do while f_sql_code() = 0
			
			 ls_upd_bank_no = ''
			 ls_upd_branch_no = ''
			 ls_upd_ac_code = ''
			 ls_upd_fis_sname = ''
			 ls_upd_currency_no = ''
			 ls_upd_pay_address = ''
			 ls_upd_swift_code = ''
			 ldec_wire_fee = 0
			 ldec_total_return = 0
			 ll_count_f = 0
			 ls_upd_intermediary_bank_no = ''
			 ls_upd_intermediary_ac_code = ''
			
			 ll_row ++
			 
			 ll_count_h = 0
			 select count(*)
			 	into :ll_count_h
				 from fas.holder_dividend
				 where id = :ls_id ;
				 
			f_sql_code()
			
			if ll_count_h <=0 then
				f_error(ls_id + '此受益人未在fas411設定，請先設定')
			else
				if isnull(ls_holder_status) or LenA(trim(ls_holder_status)) = 0 or ls_holder_status <> 'R' then
					f_error(ls_id + '此受益人在fas411資料尚未覆核確認，請確認')
				end if 
			end if 
			 
			if ls_fund_category = '1' then
				 ldec_amount          = Round(ldec_shares * ldec_dividend_rate,ldec_amt_decimal)
			      ldec_stock_amount    = Round(ldec_shares * ldec_stock_rate,ldec_amt_decimal)
				 ldec_security_amount = Round(ldec_shares * ldec_security_rate,ldec_amt_decimal)
				 ldec_security_amount_onshore = Round(ldec_shares * ldec_security_rate_onshore,ldec_amt_decimal)
//			      ldec_interest_amount = Round(ldec_shares * ldec_interest_rate,ldec_amt_decimal)
//				 ldec_interest_amount = ldec_amount - ldec_stock_amount - ldec_security_amount - ldec_security_amount_onshore (Kent)
				 //SR01658472 配息來自於本金應不可計入海外所得 Modified by Eric
				 //ldec_interest_amount = Round(ldec_shares * ldec_dividend_rate - ldec_shares * ldec_stock_rate - ldec_shares * ldec_security_rate - ldec_shares * ldec_security_rate_onshore ,ldec_amt_decimal)
				 //ldec_interest_amount = Round(ldec_shares * ldec_interest_rate_f - ldec_shares * ldec_stock_rate - ldec_shares * ldec_security_rate - ldec_shares * ldec_security_rate_onshore  - ldec_shares * ldec_capital_rate ,ldec_amt_decimal)
				 ldec_interest_amount = Round(ldec_shares * ldec_dividend_rate - ldec_shares * ldec_stock_rate - ldec_shares * ldec_security_rate - ldec_shares * ldec_security_rate_onshore  - ldec_shares * ldec_capital_rate ,ldec_amt_decimal)
				 ldec_capital_amount = Round(ldec_shares * ldec_capital_rate,ldec_amt_decimal)
				 
				 ldec_difference = ldec_amount - ldec_stock_amount - ldec_security_amount - ldec_security_amount_onshore -ldec_interest_amount - ldec_capital_amount
				 
				 if (ldec_dividend_max = ldec_stock_rate) then 
					ldec_stock_amount = ldec_stock_amount + ldec_difference
				 elseif  (ldec_dividend_max = ldec_security_rate) then
					ldec_security_amount = ldec_security_amount + ldec_difference
				 elseif  (ldec_dividend_max = ldec_capital_rate) then
					ldec_capital_amount = ldec_capital_amount + ldec_difference
				 elseif  (ldec_dividend_max = ldec_security_rate_onshore) then
					ldec_security_amount_onshore = ldec_security_amount_onshore + ldec_difference
				 else
					if ldec_interest_rate_f > ldec_capital_rate then
						ldec_interest_amount = ldec_interest_amount + ldec_difference
					else 
						ldec_capital_amount = ldec_capital_amount + ldec_difference
				    end if
				end if
				 
//				 if (ldec_difference = 1  ) then
//					if (ldec_stock_amount > 0 ) then 
//						ldec_stock_amount = ldec_stock_amount + 1
//					elseif (ldec_security_amount > 0 ) then
//						ldec_security_amount = ldec_security_amount + 1
//					elseif (ldec_security_amount_onshore > 0 ) then
//						ldec_security_amount_onshore = ldec_security_amount_onshore + 1
//					elseif (ldec_interest_amount > 0 ) then
//						ldec_interest_amount = ldec_interest_amount + 1
//					end if
//				elseif   (ldec_difference = -1 ) then
//					if (ldec_stock_amount > 0 ) then 
//						ldec_stock_amount = ldec_stock_amount -1
//					elseif (ldec_security_amount > 0 ) then
//						ldec_security_amount = ldec_security_amount -1
//					elseif (ldec_security_amount_onshore > 0 ) then
//						ldec_security_amount_onshore = ldec_security_amount_onshore -1
//					elseif (ldec_interest_amount > 0 ) then
//						ldec_interest_amount = ldec_interest_amount -1
//					end if
//				  end if 
				  
				 
				if ls_offering_type = '2' then
					ls_pay_type = '4' 
				end if 
				 
				 
				 // 若付款方式為匯款(不受最低金額限制), 且分配基金存在於fas.holder_dividend_fund，則此受益人此檔基金不受最低金額限制
				 if ls_pay_type = '4' then
					//20161027 No002313 境內若選不受最低金額限制則皆以現金發放, 故設定ll_count_f = 1(原始程式判斷邏輯先保留)
					ll_count_f = 1
					/*
					select nvl(count(*),0)
					into :ll_count_f
					from fas.holder_dividend_fund
					where id = :ls_id
					and fund_no = :ls_fund_no ;
					
					f_sql_code()
					*/
				end if 
				 
		    elseif ls_fund_category = '2' then
			// 配合BONY第三階段, 無條件捨去改為四捨五入	
				if f_sysdate()>=ldt_prog_ctrl_date then
					 ldec_amount          = Round(ldec_shares * ldec_dividend_rate,ldec_amt_decimal)
				else
					 ldec_amount          = Truncate(ldec_shares * ldec_dividend_rate,ldec_amt_decimal)
				end if	 
			      ldec_stock_amount    = 0 
				 ldec_security_amount = 0
				 ldec_security_amount_onshore = 0
				 ldec_interest_amount = ldec_amount
		    end if 		
			// 由於配合BONY未來可能要使用原始申購幣別+基金別判斷最低申購金額			
			ldec_sum_shares = 0
			ldec_sum_amount = 0
			
			select sum(shares) into :ldec_sum_shares
				from fas.certificate
			  where fas.certificate.issue_date <= :ld_dividend_date
				 and (fas.certificate.cancel_date > :ld_dividend_date or fas.certificate.cancel_date is null)
				 and fas.certificate.fund_no = :ls_fund_no
				 and fas.certificate.shares  > 0 
				 and fas.certificate.id      = :ls_id;
			
			// 配合BONY第三階段, 無條件捨去改為四捨五入	
			if f_sysdate()>=ldt_prog_ctrl_date then
				 ldec_sum_amount = Round(ldec_sum_shares * ldec_dividend_rate,ldec_amt_decimal)
			else	 
			 	 ldec_sum_amount = Truncate(ldec_sum_shares * ldec_dividend_rate,ldec_amt_decimal)
			end if	  
			
			/* 20150525修改
			select count(*) into :ll_count2 from fas.holder_dividend_bank
				where id = :ls_id 
					and ((:ls_fund_category = '1' and onshore = 'Y' ) or
						   (:ls_fund_category = '2' and offshore = 'Y') ) ;
			*/
			
			select count(*) into :ll_count2 from fas.holder_dividend_bank
				where id = :ls_id 
					and amc_no =  :ls_amc_no;
					
			if ll_count2 = 0 and (isnull(ls_pay_type) or len(trim(ls_pay_type)) = 0)  then
				 if ls_fund_category = '1' then                 
					 ls_pay_type = ''
				 elseif ls_fund_category = '2' then
					 if (ldec_sum_amount < ldec_dividend_min )  then
						 ls_pay_type = '2' 
						 ls_fund_no_in = ls_fund_no
					 elseif (ldec_sum_amount >= ldec_dividend_min)  then 
						 ls_pay_type = '3'
						 ls_fund_no_in = ''
					 end if 
				 end if 
			else // FAS411 已建檔
				
				if ls_fund_category = '1'  and ls_offering_type = '1' then
					// 若付款方式為「匯款」或(「匯款不受最低金額限制」，基金別不存在於fas.holder_dividend_fund(只有境內基金需判斷))，配息金額低於最低配息金額, 一律轉申購回原基金
					// 2014/6/9 Anita 要求，境外基金付款方式為「匯款不受最低金額限制」，不論金額全部匯回客戶帳上(基金別不需存在fas.holder_dividend_fund
					if ((ldec_sum_amount < ldec_dividend_min ) and  ls_pay_type <> '4') or (ldec_sum_amount < ldec_dividend_min and ls_pay_type = '4' and ll_count_f = 0)  then      
						 ls_pay_type = '2' 
						 ls_fund_no_in = ls_fund_no
					 else																								  // 若轉配息基金為D31, 或配息金額高於最低配息金額, 則按客戶約定方式處理,                                                                                                      				
						 if ls_pay_type = '2' then
							 if isnull(ls_fund_no_in) then ls_fund_no_in = ls_fund_no
						 end if 
						 
						 ls_upd_pay_address = ls_pay_address
					 end if 		
				elseif ls_fund_category = '1' and ls_offering_type = '2' then
					ls_pay_type = '4'
				end if 
				
				if ls_fund_category = '2' then
					if ls_pay_type = '2' then
						if isnull(ls_fund_no_in) or len(trim(ls_fund_no_in)) = 0 then
							ls_fund_no_in = ls_fund_no
						end if 
					elseif ls_pay_type = '3' then
						if (ldec_sum_amount < ldec_dividend_min) then
							ls_pay_type = '2'
							ls_fund_no_in = ls_fund_no
						end if 
					end if 
				end if 
				
				if (ls_pay_type = '3' and ldec_sum_amount >= ldec_dividend_min) or (ls_pay_type = '4' and ll_count_f > 0) or &
					(ls_pay_type = '4' and ldec_sum_amount >= ldec_dividend_min and ls_fund_category = '1' and ls_offering_type = '1') or &
					(ls_pay_type = '4' and ls_fund_category = '1' and ls_offering_type = '2') or (ls_pay_type = '4' and ls_fund_category = '2') then
				 	//-------------------------------------
					 ls_fund_no_in = ''
					  
					  //------------------------------------------------No 001365---------------------------------------------------------------------------------------------
					  ls_sql =		"select fas.holder_dividend_bank.bank_no,"
					  ls_sql +=	"		fas.holder_dividend_bank.branch_no,"
					  ls_sql +=	"		fas.holder_dividend_bank.ac_code,"
					  ls_sql +=	"		fas.holder_dividend_bank.fis_sname,"	
					  ls_sql +=	"		fas.holder_dividend_bank.currency_no,"
					  ls_sql +=	"		fas.holder_dividend_bank.ac_name"
					  ls_sql += 	"	from fas.holder_dividend_bank "
					  ls_sql +=	"	where fas.holder_dividend_bank.id = '"+ls_id+ "' "
					  /* 2015.5.25修改
					  ls_sql +=	"		and (('" +ls_fund_category+ "' = '1' and fas.holder_dividend_bank.onshore = 'Y' ) or"
					  ls_sql +=	"				('" +ls_fund_category+ "' = '2' and fas.holder_dividend_bank.offshore = 'Y') ) "
					  */
					  ls_sql +=	"		and fas.holder_dividend_bank.amc_no = '"+ls_amc_no+ "' "
					   /* No 001635  2015/3/5 Anita
					  1、id = 990418065 fund_category = '1' and org_currency_no in (USD','CNY') --> 只能入411 境內FCY帳戶
					  2、id = 990418065 fund_category = '1' and org_currency_no NOT in (USD','CNY','NTD') --> 只能入411 境內之基金計價幣別帳戶
					  3、id = 990418065 fund_category = '1' and org_currency_no = 'NTD' --> 只能入411 境內MMA 或 NTD帳戶	 
					  4、id = 990418065 fund_category = '2' and org_currency_no <> 'NTD' --> 只能入411 境外之基金計價幣別帳戶	 
					  5、id = 990418065 fund_category = '2' and org_currency_no =  'NTD' --> 只能入411 境外之NTD/MMA帳戶
					  若不符合以上條件，產生收益分配資料，但匯款帳號為空白
					  */
					  
					  /* No 001635  2015/3/5 Anita
					  1、id = 986679361 fund_category = '2' and org_currency_no = 'ZAR' --> 只能入411 境外FCY帳戶
					  2、id = 986679361 fund_category = '2' and org_currency_no NOT in ('ZAR','NTD') --> 只能入411 境外之基金計價幣別帳戶
					  3、id = 986679361 fund_category = '2' and org_currency_no = 'NTD' --> 只能入411 境外之NTD/MMA帳戶
					  4、id = 986679361 fund_category = '1' and org_currency_no <> 'NTD' --> 只能入411 境內之FCY帳戶
					  5、id = 986679361 fund_category = '1' and org_currency_no = 'NTD' --> 只能入411 境內之之NTD/MMA帳戶
					  若不符合以上條件，產生收益分配資料，但匯款帳號為空白
					  */
					  
					  Choose Case ls_id							
						Case '990418065'
							if ls_fund_category = '1' and (ls_org_currency_no = 'USD' or ls_org_currency_no = 'CNY') then
								ls_sql +=	"		and fas.holder_dividend_bank.currency_no = 'FCY' "
							elseif ls_fund_category = '1' and ls_org_currency_no <> 'USD' and ls_org_currency_no <> 'CNY' and ls_org_currency_no <> 'NTD' then
								ls_sql +=	"		and fas.holder_dividend_bank.currency_no = '" +ls_fund_currency_no+ "' "
							elseif ls_fund_category = '1' and ls_org_currency_no = 'NTD' then
								ls_sql +=	"		and fas.holder_dividend_bank.currency_no in ('NTD','MMA') "
							elseif ls_fund_category = '2' and ls_org_currency_no <> 'NTD' then
								ls_sql +=	"		and fas.holder_dividend_bank.currency_no = '" +ls_fund_currency_no+ "' "
							elseif ls_fund_category = '2' and ls_org_currency_no = 'NTD' then
								ls_sql +=	"		and fas.holder_dividend_bank.currency_no in ('NTD','MMA') "
							end if 
							
						Case '986679361'
							if ls_fund_category = '2' and ls_org_currency_no = 'ZAR' then
								ls_sql +=	"		and fas.holder_dividend_bank.currency_no = 'FCY' "
							elseif ls_fund_category = '2' and ls_org_currency_no <> 'ZAR' and ls_org_currency_no <> 'NTD' then
								ls_sql +=	"		and fas.holder_dividend_bank.currency_no = '" +ls_fund_currency_no+ "' "
							elseif ls_fund_category = '2' and ls_org_currency_no = 'NTD' then
								ls_sql +=	"		and fas.holder_dividend_bank.currency_no in ('NTD','MMA') "
							elseif ls_fund_category = '1' and ls_org_currency_no <> 'NTD' then
								ls_sql +=	"		and fas.holder_dividend_bank.currency_no = 'FCY' "
							elseif ls_fund_category = '1' and ls_org_currency_no = 'NTD' then
								ls_sql +=	"		and fas.holder_dividend_bank.currency_no in ('NTD','MMA') "
							end if 
							
						Case else
							if ls_org_currency_no = 'NTD' then
								ls_sql +=	"		and fas.holder_dividend_bank.currency_no in ('NTD','MMA') "
							elseif ls_org_currency_no <> 'NTD' then
								ls_sql +=	"		and fas.holder_dividend_bank.currency_no in ('" +ls_org_currency_no +"' ,'FCY','MMA') "
							end if 
							
					  End Choose
					  
					  DECLARE cs_fas421_account DYNAMIC CURSOR FOR SQLSA;
					  PREPARE SQLSA FROM :ls_sql;
												
					  if f_sql_code() <> 0 and f_sql_code() <> 100 then
						  f_error('select error(1) - ' +ls_id + '-' + ls_org_currency_no + '帳號邏輯有誤，請先至fas411修改')
						  cb_1_new.Enabled = true 
						  CLOSE cs_fas421;
						  return
					  end if 
					  
					  OPEN DYNAMIC cs_fas421_account;
							FETCH cs_fas421_account
							into :ls_upd_bank_no,
									:ls_upd_branch_no,
									:ls_upd_ac_code,
									:ls_upd_fis_sname,
									:ls_upd_currency_no,
									:ls_upd_ac_name ;
							do while f_sql_code() = 0 
								
								select nvl(fas.fis_bank.swift_code,''),
										nvl(fas.holder_dividend_intermediary.intermediary_bank_no,''),
										nvl(fas.holder_dividend_intermediary.intermediary_ac_code,'')
									into :ls_upd_swift_code ,
										  :ls_upd_intermediary_bank_no,
										  :ls_upd_intermediary_ac_code
									from fas.holder_dividend_intermediary , fas.fis_bank
									where fas.holder_dividend_intermediary.intermediary_bank_no = fas.fis_bank.bank_no
										and fas.holder_dividend_intermediary.bank_no = :ls_upd_bank_no
										and fas.holder_dividend_intermediary.ac_code = :ls_upd_ac_code
										and fas.holder_dividend_intermediary.id = :ls_id
										and fas.holder_dividend_intermediary.currency_no = :ls_org_currency_no 
										and ((:ls_fund_category = '1' and fas.holder_dividend_intermediary.onshore = 'Y' ) or
													(:ls_fund_category = '2' and fas.holder_dividend_intermediary.offshore = 'Y') );
									
								f_sql_code()
								
								
								if ls_fund_category = '2' then
									ls_upd_ac_name = ''
								end if 
								
								FETCH cs_fas421_account
								into :ls_upd_bank_no,
										:ls_upd_branch_no,
										:ls_upd_ac_code,
										:ls_upd_fis_sname,
										:ls_upd_currency_no,
										:ls_upd_ac_name ;
										
							Loop
							
							 if f_sql_code() <> 0 and f_sql_code() <> 100 then
								  f_error('select error(1_account) - ' +ls_id + '-' + ls_org_currency_no + '帳號邏輯有誤，請先至fas411修改')
								  cb_1_new.Enabled = true 
								  CLOSE cs_fas421;
								  CLOSE cs_fas421_account;
								  return
							  end if 
							Close cs_fas421_account;
							
					 
					  //------------------------------------------------No 001365---------------------------------------------------------------------------------------------
					  if isnull(ls_upd_bank_no) or len(trim(ls_upd_bank_no)) = 0 then //若原始付款幣別<> ntd，但fas421無外幣資料，找ntd的資料
						  	select fas.holder_dividend_bank.bank_no,
									fas.holder_dividend_bank.branch_no,
									fas.holder_dividend_bank.ac_code,
									fas.holder_dividend_bank.fis_sname,
									fas.holder_dividend_bank.currency_no,
									fas.holder_dividend_bank.ac_name
								into :ls_upd_bank_no,
										:ls_upd_branch_no,
										:ls_upd_ac_code,
										:ls_upd_fis_sname,
										:ls_upd_currency_no,
										:ls_upd_ac_name
								from fas.holder_dividend_bank
								where  fas.holder_dividend_bank.currency_no in ('NTD','MMA') 
									and fas.holder_dividend_bank.id = :ls_id 
									and fas.holder_dividend_bank.amc_no = :ls_amc_no;
									/*
									and ((:ls_fund_category = '1' and fas.holder_dividend_bank.onshore = 'Y' ) or
												(:ls_fund_category = '2' and fas.holder_dividend_bank.offshore = 'Y') ) ;
									*/
							if f_sql_code() <> 0 and f_sql_code() <> 100 then
								f_error('select error(3) - ' +ls_id + '-' + ls_org_currency_no + '帳號邏輯有誤，請先至fas411修改')
								cb_1_new.Enabled = true 
			 					CLOSE cs_fas421;
								return
							end if 
							
							select nvl(fas.fis_bank.swift_code,''),
									nvl(fas.holder_dividend_intermediary.intermediary_bank_no,''),
									nvl(fas.holder_dividend_intermediary.intermediary_ac_code,'')
								into :ls_upd_swift_code ,
									  :ls_upd_intermediary_bank_no,
									  :ls_upd_intermediary_ac_code
								from fas.holder_dividend_intermediary , fas.fis_bank
								where fas.holder_dividend_intermediary.intermediary_bank_no = fas.fis_bank.bank_no
									and fas.holder_dividend_intermediary.bank_no = :ls_upd_bank_no
									and fas.holder_dividend_intermediary.ac_code = :ls_upd_ac_code
									and fas.holder_dividend_intermediary.id = :ls_id
									and fas.holder_dividend_intermediary.currency_no = :ls_org_currency_no 
									and ((:ls_fund_category = '1' and fas.holder_dividend_intermediary.onshore = 'Y' ) or
												(:ls_fund_category = '2' and fas.holder_dividend_intermediary.offshore = 'Y') );
								
							f_sql_code()
							
							if ls_fund_category = '2' then
								ls_upd_ac_name = ''
							end if 
					  end if 
					 //-------------------------------------
					
				end if 
				
			 end if //FAS411 建檔記錄
			
			 if (ls_pay_type = '2' and ls_fund_category = '2') or ls_pay_type = '' then
				 ls_upd_bank_no = ''
				 ls_upd_branch_no = ''
				 ls_upd_ac_code = ''
				 ls_upd_fis_sname = ''
				 ls_upd_currency_no = ''
				 ls_upd_pay_address = ''
				 ldec_wire_fee = 0
				 ls_upd_swift_code = ''
			 elseif ls_pay_type = '2' then
				
				ls_broker_currency_no = ''
				
				// 境內外幣計價基金原始付款幣別為NTD轉入自己基金, 則抓FAS194幣別為轉入基金計價幣別之帳號, 
				// 其餘轉出基金原始付款幣別為NTD, 則抓FAS194幣別為NTD之帳號，
				// 原始付款幣別為外幣, 則抓FAS194幣別為外幣之帳號
				
				select fund_master_no into :ls_fund_master_in from fas.fund where fund_no = :ls_fund_no_in;
				f_sql_code()			
				
				
				if ls_fund_category = '1' and ls_fund_currency_no <> 'NTD' and ls_org_currency_no = 'NTD' and ls_fund_master = ls_fund_master_in then 
					select currency_no 
						into: ls_broker_currency_no
						from fas.fund
						where fund_no = :ls_fund_no_in;
						
					f_sql_code()
				else
					ls_broker_currency_no = ls_org_currency_no
				end if
				 
				 SELECT bank_no, 
				       		branch_no, 
						    ac_code, 
						    fis_sname,
						    ac_name
				    INTO :ls_upd_bank_no, 
				       	   :ls_upd_branch_no, 
						   :ls_upd_ac_code ,
						   :ls_upd_fis_sname,
						   :ls_upd_ac_name
				  FROM fas.broker_account
				 WHERE fund_no		= :ls_fund_no_in	AND
						 broker_no	= '999' and
						 currency_no = :ls_broker_currency_no;
				
				 ls_upd_currency_no = 'NTD'
				 ls_upd_pay_address = ''
				 ldec_wire_fee = 0
			 end if 
			 
			 if ls_fund_category = '1' and ldec_stock_amount + ldec_interest_amount + ldec_security_amount + ldec_security_amount_onshore + ldec_capital_amount > 0 and (ls_pay_type = '3' or ls_pay_type = '4') then
				 if ls_fund_no = 'D44' then
					ldec_wire_fee = 0 
				else
//					ldec_wire_fee = wf_wire_fee_b(ls_id , ls_fund_no , ls_upd_bank_no , ls_upd_branch_no , ldec_amount)
					ldec_wire_fee	= wf_wire_fee_b( ls_id, ls_fund_no, ls_upd_bank_no, ldec_amount , ls_org_currency_no,'N')
				end if
				 
			 elseif ls_fund_category = '1' and ldec_stock_amount + ldec_interest_amount + ldec_security_amount +ldec_security_amount_onshore + ldec_capital_amount <= 0 and (ls_pay_type = '3' or ls_pay_type = '4') then
				 ldec_wire_fee = 0
				 
			elseif ls_pay_type ='1' and ls_fund_category = '1' then
				ldec_wire_fee = 34
				
			elseif ls_pay_type = '2' and ldec_stock_amount + ldec_interest_amount + ldec_security_amount +ldec_security_amount_onshore + ldec_capital_amount > 0 and ls_fund_category = '1' then
				//ldec_wire_fee = wf_wire_fee_b(ls_id , ls_fund_no , ls_upd_bank_no , ls_upd_branch_no , ldec_amount)
				ldec_wire_fee	= wf_wire_fee_b( ls_id, ls_fund_no, ls_upd_bank_no, ldec_amount , ls_org_currency_no,'N')
		    end if 
			 
			 ldec_total_return = ldec_amount - ldec_wire_fee
			 
			 select count(*)
			   into :ll_count1
				from fas.dividend_detail
			  where fund_no = :ls_fund_no
			      and dividend_date = :ld_dividend_date
				 and id = :ls_id
				 and org_currency_no = :ls_org_currency_no
				 and dividend_type = :ls_dividend_type
				 and ( bel_cus_no = :ls_bel_cus_no or :ls_bel_cus_no is null );
				 
			 if ll_count1 > 0 then
				 f_error('ID = ' +ls_id+ '已存在，請確認')
				cb_1_new.Enabled = true 
			 	CLOSE cs_fas421;
				 return
			 end if 
			 
			  /* No 001635 Anita
			  FAS421產生資原，付款方式若為匯款或匯款(不受金額限制)，增加檢查原付款幣別，
			  若原始付款幣非台幣，但匯入帳號幣別為台幣時，出示警語，再往下執行
			  */
			  
			  if ls_pay_type = '3' or ls_pay_type = '4' then
				if ls_org_currency_no <> 'NTD' and ls_upd_currency_no = 'NTD' then
					f_error('ID = ' +ls_id+ '原始付款幣別非NTD，收益分配匯入銀行為台幣，請確認')
				end if
			end if 
			
			ll_id_seq=0
			
			select max(id_seq)
			   into :ll_id_seq
			  from fas.dividend_detail
			where fund_no = :ls_fund_no
			    and dividend_date = :ld_dividend_date
			    and org_currency_no = :ls_org_currency_no
			    and id = :ls_id
			    and dividend_type = :ls_dividend_type;
			  //判斷後 kent
			if (f_sql_code()=0 and not isnull(ls_bel_cus_no) and  ls_fund_category = '2' and old_fund_no <> ls_fund_no ) or  f_sql_code()<>0 or isnull(ll_id_seq) then
				ll_id_seq=0
				old_fund_no = ls_fund_no
			end if
			
			ll_id_seq++

			
			 Insert into fas.dividend_detail
						  (  fund_no               		, dividend_year      				, id                 							, shares ,
							 pay_type              		, sname              					, bank_no            						, branch_no , 
							 ac_code               		, dividend_date      				, amount             						, org_currency_no, 
							 emp_no                		, last_modified      				, fund_no_in         						, currency_no, 
							 pay_address           		, wire_fee           					, stock_amount      					, interest_amount,
							 security_amount       	, total_return       					, ac_name            					, intermediary_swift_code,
							 dividend_type            	, intermediary_bank_no			 , intermediary_ac_code 				, security_amount_onshore,
							 bel_cus_no                   , id_seq                       , capital_amount ) //SR01658472 配息來自於本金應不可計入海外所得 Modified by Eric
							
				 Values ( :ls_fund_no           		, :ls_year           					, :ls_id             						, :ldec_shares, 
							 :ls_pay_type          		, :ls_upd_fis_sname  				, :ls_upd_bank_no   					, :ls_upd_branch_no,
							 :ls_upd_ac_code       	, :ld_dividend_date  				, :ldec_amount       					, :ls_org_currency_no, 
							 :gs_id						, sysdate            					, :ls_fund_no_in     					, :ls_upd_currency_no, 
							 :ls_upd_pay_address		, :ldec_wire_fee    				, :ldec_stock_amount 				, :ldec_interest_amount,
							 :ldec_security_amount	, :ldec_total_return				 , :ls_upd_ac_name    				, :ls_upd_swift_code,
							 :ls_dividend_type 			, :ls_upd_intermediary_bank_no , :ls_upd_intermediary_ac_code  , :ldec_security_amount_onshore,
							 :ls_bel_cus_no              , :ll_id_seq                   , :ldec_capital_amount); //SR01658472 配息來自於本金應不可計入海外所得 Modified by Eric
							
			
			 if f_sql_code() < 0 then
				 f_error(ls_stage + "(6)")
				 cb_1_new.Enabled = true 
			 	CLOSE cs_fas421;
				 return 
			 elseif f_sql_code() = 0 then
				 ll_count ++
			 end if
				
			
			 FETCH cs_fas421
			  INTO :ls_org_currency_no,
					 :ls_id,
					 :ldec_shares,
					 :ls_pay_type,
					 :ls_fund_no_in,
				     :ls_pay_address,
				 	:ls_holder_status,
					 :ls_fund_currency_no,
				     :ls_fund_category,
					 :ldec_dividend_min,
					 :ldec_amt_decimal,
					 :ls_fund_master,
					 :ls_bel_cus_no;
			 
			 
		 loop
		
		 if f_sql_code() < 0 then
			 f_error(ls_stage + "(7)")
			 cb_1_new.Enabled = true 
			 CLOSE cs_fas421;
			 return 
		 end if
		
		 CLOSE cs_fas421;
	 end if  

next 

messagebox('注意','共新增 ' +string(ll_count) +' 筆資料')
commit;
cb_1_new.Enabled = true 
SetPointer(Arrow!)