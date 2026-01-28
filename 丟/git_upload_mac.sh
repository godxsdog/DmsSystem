#!/bin/bash

# 1. Change Directory
cd /Users/kaichanghuang/Documents/Phoenix Code/DmsSystem/丟

# 2. Get Date and Time (Format: YYYYMMDD_HHMMSS)
full_datetime=$(date +"%Y%m%d_%H%M%S")

# 3. Git Commands
git add .
git commit -m "$full_datetime"
git push -u origin