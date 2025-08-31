@echo off
echo Building TypoZap for Windows...
echo.

echo Restoring NuGet packages...
dotnet restore
if %ERRORLEVEL% NEQ 0 (
    echo Failed to restore packages
    pause
    exit /b 1
)

echo.
echo Building in Release mode...
dotnet build -c Release
if %ERRORLEVEL% NEQ 0 (
    echo Build failed
    pause
    exit /b 1
)

echo.
echo Build completed successfully!
echo Output files are in: bin\Release\net7.0-windows\
echo.
echo To run the application:
echo   dotnet run -c Release
echo.
pause
