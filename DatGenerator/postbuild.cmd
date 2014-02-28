rem Post-build Event Command Line:
rem "$(ProjectDir)postbuild.cmd" "$(DevEnvDir)" $(TargetFileName) $(ProjectName)

rem %1..\..\Sdk\v2.0\Bin\gacutil /i %2
rem %1..\..\Sdk\v2.0\Bin\tlbexp %2
rem %SystemRoot%\Microsoft.NET\Framework\v2.0.50727\regasm /codebase %2
rem regedit /S ..\..\%3.reg
