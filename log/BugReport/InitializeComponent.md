### 问题描述:
WFP项目中,xaml文件有一个对应的xaml.cs文件,在他的构造函数中,显示在当前上下文中无法找到InitializeComponent()函数,如果在代码中直接引用了XAML元素,还会出现找不到找个元素的情况.

### 解决方案:
方法1：可能你的文件是copy的别人的代码，所以先仔细检查每个命名空间的名称，类的名称在xaml和cs文件中是否一致。（我的错误就是这样产生的，在xaml文件中命名空间与窗体的不一致）

方法2：修改.csproj文件(此方法是对那些需要用VS2008编译运行vs2010或expression blend4生成的项目来说的)

把：<Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />

替换成:
    <Import Project="$(MSBuildBinPath)\Microsoft.CSharp.targets"/>
    <Import Project="$(MSBuildBinPath)\Microsoft.WinFX.targets"/>
然后再MSBuild下试试

那么这个InitializeComponent方法在哪儿呢？这个是在系统自己生成的文件MainWindow.g.i.cs文件中，这个文件在资源管理器中不显示，在项目文件夹/obj/Debug下，如果上述方案还不能解决，可能是这个文件中没有InitializeComponent（）了，这时可以从其他项目中把这个方法复制过来。


### 参考
https://blog.csdn.net/ccx_john/article/details/17042325

