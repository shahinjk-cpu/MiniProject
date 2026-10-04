@echo off
title PetShop Sayti Basladilir...
echo ==============================================
echo   PetShop Sayti ve Admin Paneli Ise Salinir
echo ==============================================
cd /d "C:\Users\sahin\PetShopApp"
timeout /t 2 /nobreak >nul
start http://localhost:5000
dotnet run --urls=http://localhost:5000
pause
