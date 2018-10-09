### 问题描述
任务失败，因为未找到“resgen.exe”，或未安装正确的 Microsoft Windows SDK。
任务正在注册表项 
HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx35Tools-x86 
的 
InstallationFolder 值中所指定位置下的“bin”子目录中查找“resgen.exe”。
### 系统提示的解决方案
通过执行下列操作之一可以解决此问题: 
1) 安装 Microsoft Windows SDK。
2) 安装 Visual Studio 2010。 
3) 手动向正确的位置设置上面的注册表项。
4) 将正确的位置传入任务的“ToolPath”参数中。	

### 最终解决方案
这个是因为缺少.Net3,5开发包导致的,这是.Net的兼容性包袱了….应该很难改,只能通过安装来解决
