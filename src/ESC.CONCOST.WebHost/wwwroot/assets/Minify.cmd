@ECHO OFF
rem curl -X POST -s --data-urlencode input@style.css https://cssminifier.com/raw > style.min.css
rem curl -X POST -s --data-urlencode input@slk.css https://cssminifier.com/raw > slk.min.css

del bundle.css

type bootstrap\4.5.3\css\bootstrap.min.css >> bundle.css
echo. >> bundle.css
echo. >> bundle.css
type font-awesome\5.15.1\css\all.css >> bundle.css
echo. >> bundle.css
echo. >> bundle.css
type app.css >> bundle.css

del bundle.js

type jquery\jquery-3.5.1.min.js >> bundle.js
echo. >> bundle.js
echo. >> bundle.js
type bootstrap\4.5.3\js\bootstrap.bundle.min.js >> bundle.js
echo. >> bundle.js
echo. >> bundle.js
type sweetalert2\sweetalert2.js >> bundle.js
echo. >> bundle.js
echo. >> bundle.js
type sweetalert2\sweetalert2-blazor.js >> bundle.js
echo. >> bundle.js
echo. >> bundle.js
type app.js >> bundle.js

PAUSE