import os

# --- 腳本設定 ---

# 1. 要建立的資料夾名稱清單
# (我已根據您提供的圖片手動辨識並轉為大寫)
folder_names = [
    "FAS11", "FAS12", "FAS14", "FAS15", "FAS16", "FAS17", "FAS18", "FAS19",
    "FAS21", "FAS22", "FAS23", "FAS24", "FAS25", "FAS26", "FAS27", "FAS28",
    "FAS29", "FAS31", "FAS32", "FAS33", "FAS41", "FAS42", "FAS3", "FAS5",
    "FAS61", "FAS62", "FAS63", "FAS64", "FAS65", "FAS66", "FAS67", "FAS68",
    "FAS69", "FAS71", "FAS81", "FAS89", "FAS9", "FASACC", "FASCHECK",
    "FASDD", "FASLIB", "RISMAIL", "CPBUSINESS", "CPLIB", "CPSECURITY",
    "PFCAPSRV", "PFCWSRV", "PFCOLD", "PFCUTIL", "PFCWNSRV", "PFEAPSRV",
    "PFEDWSRV", "PFEIMAIN", "PFEUTIL", "PFEWSRV"
]

# 根據您的範例 "FAS13"，手動加入 (因為圖片中沒有FAS13)
if "FAS13" not in folder_names:
    folder_names.append("FAS13")
    print("已根據您的範例要求，額外加入 'FAS13'。")

# 2. 建立一個主資料夾來存放所有資料夾
# (您可以將 "all_project_folders" 改成您想要的名稱)
base_output_dir = "all_project_folders"

# --- 執行 ---

print(f"準備在 '{base_output_dir}' 中建立資料夾...")

# 建立基礎資料夾 (如果它不存在)
os.makedirs(base_output_dir, exist_ok=True)
print(f"主資料夾 '{base_output_dir}' 已確認。")

count = 0
# 迴圈清單並建立所有子資料夾
print("開始建立子資料夾...")
for name in sorted(folder_names): # 使用 sorted() 讓建立順序更整齊
    try:
        # 組合出完整的資料夾路徑
        folder_path = os.path.join(base_output_dir, name)
        
        # 建立資料夾，exist_ok=True 表示如果資料夾已存在也不會報錯
        os.makedirs(folder_path, exist_ok=True)
        print(f"  [成功] 已建立: {folder_path}")
        count += 1
    except Exception as e:
        print(f"  [失敗] 建立 {name} 時發生錯誤: {e}")

print(f"\n--- 任務完成 ---")
print(f"總共成功建立了 {count} 個資料夾。")