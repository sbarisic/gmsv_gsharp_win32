set PATH=E:\D\dmd2\windows\bin;C:\Program Files (x86)\Windows Kits\8.0\\bin;%PATH%
dmd -g -debug -X -Xf"Debug\gmsv_d_win32.json" -deps="Debug\gmsv_d_win32.dep" -c -of"Debug\gmsv_d_win32.obj" dll.def dllmain.d
if errorlevel 1 goto reportError

set LIB="E:\D\dmd2\windows\bin\..\lib";\dm\lib
echo. > Debug\gmsv_d_win32.build.lnkarg
echo "Debug\gmsv_d_win32.obj","Debug\gmsv_d_win32.dll_cv","Debug\gmsv_d_win32.map",user32.lib+ >> Debug\gmsv_d_win32.build.lnkarg
echo kernel32.lib,dll.def/NOMAP/CO/NOI >> Debug\gmsv_d_win32.build.lnkarg

"C:\Program Files (x86)\VisualD\pipedmd.exe" -deps Debug\gmsv_d_win32.lnkdep E:\D\dmd2\windows\bin\link.exe @Debug\gmsv_d_win32.build.lnkarg
if errorlevel 1 goto reportError
if not exist "Debug\gmsv_d_win32.dll_cv" (echo "Debug\gmsv_d_win32.dll_cv" not created! && goto reportError)
echo Converting debug information...
"C:\Program Files (x86)\VisualD\cv2pdb\cv2pdb.exe" "Debug\gmsv_d_win32.dll_cv" "Debug\gmsv_d_win32.dll"
if errorlevel 1 goto reportError

if errorlevel 1 goto reportError
COPY Debug\gmsv_d_win32.dll E:\Projects\gmsv_gsharp_win32\bin\gmsv_d_win32.dll

if not exist "Debug\gmsv_d_win32.dll" (echo "Debug\gmsv_d_win32.dll" not created! && goto reportError)

goto noError

:reportError
echo Building Debug\gmsv_d_win32.dll failed!

:noError
