$formsVersion = "3.4.0.1008975"

echo "  <<<< WARNING >>>>> You need to launch 2 times this script to make sure AssemblyInfo.cs is correctly generated..."

$netstandardProject = ".\Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj"
$droidProject = ".\Sharpnado.Presentation.Forms.Droid\Sharpnado.Presentation.Forms.Droid.csproj"
$iosProject = ".\Sharpnado.Presentation.Forms.iOS\Sharpnado.Presentation.Forms.iOS.csproj"

$hlvProject = ".\Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.HLV.csproj"
$droidHLVProject = ".\Sharpnado.Presentation.Forms.Droid\Sharpnado.Presentation.Forms.Droid.HLV.csproj"
$iosHLVProject = ".\Sharpnado.Presentation.Forms.iOS\Sharpnado.Presentation.Forms.iOS.HLV.csproj"

echo "  Setting Xamarin.Forms version to $formsVersion"

$findXFVersion = '(Xamarin.Forms">\s+<Version>)(.+)(</Version>)'
$replaceString = "`$1$formsVersion`$3"

(Get-Content $netstandardProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $netstandardProject
(Get-Content $droidProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $droidProject
(Get-Content $iosProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $iosProject

(Get-Content $hlvProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $hlvProject
(Get-Content $droidHLVProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $droidHLVProject
(Get-Content $iosHLVProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $iosHLVProject



echo "##################################"
echo "# Sharpnado.Presentation.Forms"
echo "##################################"

echo "  building Sharpnado.Presentation.Forms solution -- normal mode"
msbuild .\Sharpnado.Presentation.Forms.sln /t:Clean,Restore,Build /p:Configuration=Release > build.txt

echo "  building Android9"
msbuild .\Sharpnado.Presentation.Forms.Droid\Sharpnado.Presentation.Forms.Droid.csproj /t:Clean,Restore,Build /p:Configuration=ReleaseAndroid9.0 > build.Android9.txt

$version = (Get-Item Sharpnado.Presentation.Forms\bin\Release\netstandard2.0\Sharpnado.Presentation.Forms.dll).VersionInfo.FileVersion

echo "  packaging Sharpnado.Presentation.Forms.nuspec (v$version)"
nuget pack .\Sharpnado.Presentation.Forms.nuspec -Version $version


echo "########################################"
echo "# Sharpnado.Forms.HorizontalListView"
echo "########################################"

echo "  building Sharpnado.Presentation.Forms.HLV solution -- only HorizontalListView"
msbuild .\Sharpnado.Presentation.Forms.HLV.sln /t:Clean,Restore,Build /p:Configuration=Release > build.HLV.txt

echo "  building Android9 -- only HorizontalListView"
msbuild .\Sharpnado.Presentation.Forms.Droid\Sharpnado.Presentation.Forms.Droid.HLV.csproj /t:Clean,Restore,Build /p:Configuration=ReleaseAndroid9.0 > build.Android9.HLV.txt

$version = (Get-Item Sharpnado.Presentation.Forms\bin\HLVRelease\netstandard2.0\Sharpnado.Presentation.Forms.dll).VersionInfo.FileVersion

echo "  packaging Sharpnado.Forms.HorizontalListView.nuspec (v$version)"
nuget pack .\Sharpnado.Forms.HorizontalListView.nuspec -Version $version
