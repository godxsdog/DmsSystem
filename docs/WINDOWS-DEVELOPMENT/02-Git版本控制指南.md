# Git 版本控制指南

> 📋 **適用對象**：所有參與開發的團隊成員

## 🎯 基本概念

### 為什麼需要版本控制？

- **追蹤變更**：記錄所有程式碼變更歷史
- **協作開發**：多人可以同時開發而不衝突
- **版本管理**：可以回到任何歷史版本
- **分支管理**：可以同時開發多個功能

## 🔄 標準工作流程

### 1. 取得最新程式碼

```powershell
# 切換到 main 分支
git checkout main

# 取得遠端最新變更
git pull origin main
```

### 2. 建立功能分支

```powershell
# 建立並切換到新分支
git checkout -b feature/功能名稱

# 範例：
git checkout -b feature/add-report-export
git checkout -b fix/stock-balance-calculation
git checkout -b refactor/file-parser
```

**分支命名規範：**
- `feature/` - 新功能
- `fix/` - 修復 bug
- `refactor/` - 重構
- `docs/` - 文件變更
- `test/` - 測試相關

### 3. 開發並提交

```powershell
# 查看變更狀態
git status

# 查看變更內容
git diff

# 加入變更到暫存區
git add .

# 或加入特定檔案
git add DmsSystem.Api/Controllers/NewController.cs

# 提交變更
git commit -m "feat: 描述變更內容"
```

### 4. 推送到遠端

```powershell
# 首次推送（建立遠端分支）
git push -u origin feature/功能名稱

# 後續推送
git push
```

### 5. 建立 Pull Request

1. 在 Git 平台（GitHub/GitLab）建立 Pull Request
2. 選擇 `main` 作為目標分支
3. 填寫 PR 說明：
   - 變更內容
   - 測試結果
   - 相關 Issue 編號
4. 等待 Code Review
5. 通過後合併

### 6. 合併後清理

```powershell
# 切換回 main
git checkout main

# 取得最新變更（包含你的 PR）
git pull origin main

# 刪除本地分支
git branch -d feature/功能名稱

# 刪除遠端分支（如果已合併）
git push origin --delete feature/功能名稱
```

## 📝 Commit 訊息規範

### 格式

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Type 類型

| 類型 | 說明 | 範例 |
|------|------|------|
| `feat` | 新功能 | `feat(api): 新增資料匯出功能` |
| `fix` | 修復 bug | `fix: 修正股票餘額計算錯誤` |
| `docs` | 文件變更 | `docs: 更新 API 文件` |
| `style` | 程式碼格式 | `style: 統一縮排格式` |
| `refactor` | 重構 | `refactor: 重構檔案解析邏輯` |
| `test` | 測試 | `test: 新增 Service 單元測試` |
| `chore` | 建置或工具 | `chore: 更新 NuGet 套件版本` |

### Scope（可選）

- `api` - API 相關
- `frontend` - 前端相關
- `database` - 資料庫相關
- `config` - 設定相關

### 範例

#### 簡單變更
```
feat(api): 新增股東會資料查詢 API
```

#### 詳細變更
```
feat(api): 新增股東會資料查詢 API

- 實作 DataViewController
- 加入分頁功能
- 更新 Repository 介面加入 GetAllAsync 方法

Closes #123
```

#### 修復 Bug
```
fix: 修正股票餘額計算錯誤

修正當 Shares 為負數時計算錯誤的問題

Fixes #456
```

## 🔀 處理衝突

### 當 Pull Request 有衝突時

```powershell
# 1. 取得最新 main
git checkout main
git pull origin main

# 2. 切換回你的分支
git checkout feature/your-feature

# 3. 合併 main 到你的分支
git merge main

# 4. 解決衝突
# 編輯有衝突的檔案，移除衝突標記
# 選擇保留的程式碼

# 5. 標記衝突已解決
git add .
git commit -m "merge: 解決與 main 的衝突"

# 6. 推送
git push
```

### 使用 Rebase（進階）

```powershell
# 在功能分支上
git rebase main

# 如果有衝突，解決後
git add .
git rebase --continue

# 推送（需要 force push）
git push --force-with-lease
```

## 📋 日常檢查清單

### 開始工作前
- [ ] 取得最新程式碼：`git pull origin main`
- [ ] 建立功能分支：`git checkout -b feature/xxx`

### 開發中
- [ ] 經常提交變更（小的、邏輯完整的變更）
- [ ] 使用規範的 commit 訊息
- [ ] 不要提交敏感資訊（密碼、API Key）

### 提交前
- [ ] 測試功能是否正常
- [ ] 檢查是否有未提交的變更：`git status`
- [ ] 確認 commit 訊息格式正確

### 推送後
- [ ] 建立 Pull Request
- [ ] 填寫完整的 PR 說明
- [ ] 通知團隊成員進行 Review

## ⚠️ 禁止事項

1. **不要直接推送到 main**
   - 必須透過 Pull Request
   - 需要 Code Review

2. **不要提交敏感資訊**
   - 密碼、API Key
   - 個人設定檔
   - 使用 `.gitignore` 排除

3. **不要強制推送 main**
   - `git push --force` 會破壞歷史
   - 只允許在功能分支使用（謹慎）

4. **不要提交大型檔案**
   - 使用 Git LFS 或外部儲存

## 🔧 常用指令

```powershell
# 查看狀態
git status

# 查看變更
git diff

# 查看歷史
git log --oneline

# 查看分支
git branch -a

# 切換分支
git checkout branch-name

# 建立並切換分支
git checkout -b new-branch

# 合併分支
git merge branch-name

# 取消暫存
git reset HEAD file-name

# 取消變更
git checkout -- file-name

# 查看遠端
git remote -v
```

## 🎓 最佳實踐

1. **小步提交**：頻繁提交小的、邏輯完整的變更
2. **清晰的訊息**：commit 訊息要清楚說明變更內容
3. **測試後提交**：確保功能正常後再提交
4. **定期同步**：經常從 main 取得最新變更
5. **Code Review**：所有變更都要經過 Review

## 🔗 相關資源

- [Git 官方文件](https://git-scm.com/doc)
- [GitHub Flow](https://guides.github.com/introduction/flow/)
- [Conventional Commits](https://www.conventionalcommits.org/)

---

**遵循這些規範可以讓團隊協作更順暢！**

