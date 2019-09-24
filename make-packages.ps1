$formsVersion = "3.4.0.1039999"

echo "  <<<< WARNING >>>>> You need to launch 2 times this script to make sure Xamarin.Forms version was correctly resolved..."

$netstandardProject = ".\Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj"
$hlvProject = ".\Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.HorizontalListView.csproj"
$droidProject = ".\Sharpnado.Presentation.Forms.Droid\Sharpnado.Presentation.Forms.Droid.csproj"
$iosProject = ".\Sharpnado.Presentation.Forms.iOS\Sharpnado.Presentation.Forms.iOS.csproj"

echo "  Setting Xamarin.Forms version to $formsVersion"

$findXFVersion = '(Xamarin.Forms">\s+<Version>)(.+)(</Version>)'
$replaceString = "`$1 $formsVersion `$3"

(Get-Content $netstandardProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $netstandardProject
(Get-Content $hlvProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $hlvProject
(Get-Content $droidProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $droidProject
(Get-Content $iosProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $iosProject

echo "  building Sharpnado.Presentation.Forms solution -- normal mode"
msbuild .\Sharpnado.Presentation.Forms.sln /t:Clean,Restore,Build /p:Configuration=Release > build.txt

$version = (Get-Item Sharpnado.Presentation.Forms\bin\Release\netstandard2.0\Sharpnado.Presentation.Forms.dll).VersionInfo.FileVersion

echo "  packaging Sharpnado.Presentation.Forms.nuspec (v$version)"
nuget pack .\Sharpnado.Presentation.Forms.nuspec -Version $version > $null

echo "  backuping Sharpnado.Presentation.Forms.csproj"
cp Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj.bak
echo "  renaming Sharpnado.Presentation.Forms.HorizontalListView.csproj to Sharpnado.Presentation.Forms.csproj"
cp Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.HorizontalListView.csproj Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj 

echo "  building Sharpnado.Presentation.Forms solution -- only HorizontalListView"
msbuild .\Sharpnado.Presentation.Forms.sln /t:Clean,Restore,Build /p:Configuration=Release > build.HorizontalListView.txt
echo "  packaging Sharpnado.Forms.HorizontalListView.nuspec (v$version)"
nuget pack .\Sharpnado.Forms.HorizontalListView.nuspec -Version $version > $null

echo "  restoring Sharpnado.Presentation.Forms.csproj"
cp Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj.bak Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj
rm Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj.bak