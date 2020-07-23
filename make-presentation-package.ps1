$formsVersion = "3.6.0.220655"

$netstandardProject = ".\Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj"
$droidProject = ".\Sharpnado.Presentation.Forms.Droid\Sharpnado.Presentation.Forms.Droid.csproj"
$iosProject = ".\Sharpnado.Presentation.Forms.iOS\Sharpnado.Presentation.Forms.iOS.csproj"


rm *.txt

echo "  Setting Xamarin.Forms version to $formsVersion"

$findXFVersion = '(Xamarin.Forms">\s+<Version>)(.+)(<\/Version>)'
$replaceString = "`$1 $formsVersion `$3"

(Get-Content $netstandardProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $netstandardProject
(Get-Content $droidProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $droidProject
(Get-Content $iosProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $iosProject



echo "##################################"
echo "# Sharpnado.Presentation.Forms"
echo "##################################"

echo "  cleaning Sharpnado.Presentation.Forms solution"
$errorCode = msbuild .\Sharpnado.Presentation.Forms.sln /t:Clean /p:Configuration=Release > clean.txt
if ($errorCode -gt 0)
{
    echo "  Error while cleaning solution, see clean.txt for infos"
    return 1
}


echo "  restoring nuget packages"
$errorCode = msbuild .\Sharpnado.Presentation.Forms.sln /t:Restore > restore.txt
if ($errorCode -gt 0)
{
    echo "  Error while restoring packages, see restore.txt for infos"
    return 2
}


echo "  building Sharpnado.Presentation.Forms solution"
$errorCode = msbuild .\Sharpnado.Presentation.Forms.sln /t:Build /p:Configuration=Release > build.txt
if ($errorCode -gt 0)
{
    echo "  Error while building solution, see build.txt for infos"
    return 3
}


echo "  building Android9 version"
$errorCode = msbuild .\Sharpnado.Presentation.Forms.Droid\Sharpnado.Presentation.Forms.Droid.csproj /t:Clean,Restore,Build /p:Configuration=ReleaseAndroid9.0 > build.Android9.txt
if ($errorCode -gt 0)
{
    echo "  Error while building Android9 version, see build.Android9.txt for infos"
    return 4
}


$version = (Get-Item Sharpnado.Presentation.Forms\bin\Release\netstandard2.0\Sharpnado.Presentation.Forms.dll).VersionInfo.FileVersion

echo "  packaging Sharpnado.Presentation.Forms.nuspec (v$version)"
nuget pack Sharpnado.Presentation.Forms.nuspec -Version $version
