$formsVersion = "3.6.0.220655"

$hlvProject = ".\Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.HLV.csproj"
$droidHLVProject = ".\Sharpnado.Presentation.Forms.Droid\Sharpnado.Presentation.Forms.Droid.HLV.csproj"
$iosHLVProject = ".\Sharpnado.Presentation.Forms.iOS\Sharpnado.Presentation.Forms.iOS.HLV.csproj"


rm *.txt

echo "  Setting Xamarin.Forms version to $formsVersion"

$findXFVersion = '(Xamarin.Forms">\s+<Version>)(.+)(<\/Version>)'
$replaceString = "`$1 $formsVersion `$3"

(Get-Content $hlvProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $hlvProject
(Get-Content $droidHLVProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $droidHLVProject
(Get-Content $iosHLVProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $iosHLVProject


echo "########################################"
echo "# Sharpnado.Forms.HorizontalListView"
echo "########################################"

echo "  building Sharpnado.Presentation.Forms.HLV solution -- only HorizontalListView"
$errorCode = msbuild .\Sharpnado.Presentation.Forms.HLV.sln /t:Clean,Restore,Build /p:Configuration=Release > build.HLV.txt
if ($errorCode -gt 0)
{
    echo "  Error while building HorizontalListView version, see build.HLV.txt for infos"
    return 5
}


echo "  building Android9 -- only HorizontalListView"
$errorCode = msbuild .\Sharpnado.Presentation.Forms.Droid\Sharpnado.Presentation.Forms.Droid.HLV.csproj /t:Clean,Restore,Build /p:Configuration=ReleaseAndroid9.0 > build.Android9.HLV.txt
if ($errorCode -gt 0)
{
    echo "  Error while building Android9 HorizontalListView version, see build.Android9.HLV.txt for infos"
    return 6
}


$version = (Get-Item Sharpnado.Presentation.Forms\bin\HLVRelease\netstandard2.0\Sharpnado.Presentation.Forms.dll).VersionInfo.FileVersion

echo "  packaging Sharpnado.Forms.HorizontalListView.nuspec (v$version)"
nuget pack .\Sharpnado.Forms.HorizontalListView.nuspec -Version $version
