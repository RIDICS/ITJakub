/// <binding BeforeBuild='yarn-runtime' Clean='clean' ProjectOpened='watch, yarn-runtime' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require("gulp"),
  rimraf = require("rimraf"),
  concat = require("gulp-concat"),
  cssmin = require("gulp-cssmin"),
  less = require("gulp-less"),
  lessToScss = require("gulp-less-to-scss"),
  sass = require("gulp-dart-sass"),
  watch = require("gulp-watch"),
  sourcemaps = require("gulp-sourcemaps"),
  uglify = require("gulp-uglify"),
  ts = require("gulp-typescript"),
  yarn = require("gulp-yarn");
var tsProject = ts.createProject("tsconfig.json");

var paths = {
    webroot: "./wwwroot/"
};

paths.less = paths.webroot + "css/**/*.less";
paths.css = paths.webroot + "css/**/*.css";
paths.js = paths.webroot + "js/**/*.js";
paths.jsMap = paths.webroot + "js/**/*.js.map";
paths.ts = paths.webroot + "js/**/*.ts";
paths.areaLess = paths.webroot + "Areas/*/css/**/*.less";
paths.areaCss = paths.webroot + "Areas/*/css/**/*.css";
paths.areaJs = paths.webroot + "Areas/*/js/**/*.js";
paths.areaJsMap = paths.webroot + "Areas/*/js/**/*.js.map";
paths.areaTs = paths.webroot + "Areas/*/js/**/*.ts";


// Clean-up

gulp.task("clean:css", function (cb) {
    rimraf(paths.css, cb);
});

gulp.task("clean:css_areas", function (cb) {
    rimraf(paths.areaCss, cb);
});

gulp.task("clean:js", function (cb) {
    rimraf(paths.js, cb);
});

gulp.task("clean:js_areas", function (cb) {
    rimraf(paths.areaJs, cb);
});

gulp.task("clean:jsmap", function (cb) {
    rimraf(paths.jsMap, cb);
});

gulp.task("clean:jsmap_areas", function (cb) {
    rimraf(paths.areaJsMap, cb);
});

gulp.task("clean", gulp.parallel("clean:css", "clean:css_areas", "clean:js", "clean:js_areas", "clean:jsmap", "clean:jsmap_areas"));


// Less build

gulp.task("build:less_root", function () {
    return gulp.src(paths.less)
        .pipe(sourcemaps.init())
        .pipe(less({
            relativeUrls: true
        }).on("error", (err) => {
            console.error("Error building LESS:", err.message);
            process.exit(1);
        }))
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "css"));
});

gulp.task("build:less_areas", function () {
    return gulp.src(paths.areaLess)
        .pipe(sourcemaps.init())
        .pipe(less({
            relativeUrls: true
        }).on("error", (err) => {
            console.error("Error building LESS:", err.message);
            process.exit(1);
        }))
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas"));
});

gulp.task("build:less", gulp.parallel("build:less_root", "build:less_areas"));

gulp.task("watch:less_root", function() {
    return gulp.watch(paths.less, ["build:less_root"]);
});

gulp.task("watch:less_areas", function () {
    return gulp.watch(paths.areaLess, ["build:less_areas"]);
});

gulp.task("watch:less", gulp.parallel("watch:less_root", "watch:less_areas"));


// Sass build
gulp.task("convert:sass_colors", function() {
    return gulp.src(paths.webroot + "css/ITJakub.Colors-selected.less")
        .pipe(lessToScss())
        .pipe(gulp.dest(paths.webroot + "css"));
});

gulp.task("build:sass_lib_list", function() {
    return gulp.src([
            paths.webroot + "css/lib.loading-visualization.scss"
        ])
        .pipe(sass().on("error", sass.logError))
        .pipe(gulp.dest(paths.webroot + "css"));
});

gulp.task("build:sass_lib", gulp.series("convert:sass_colors", "build:sass_lib_list"));


// TypeScript build
gulp.task("build:ts", function () {
    var tsResult = tsProject.src()
        .pipe(sourcemaps.init())
        .pipe(tsProject());

    return tsResult.js
        .pipe(sourcemaps.write(".", { sourceRoot: "." }))
        .pipe(gulp.dest(paths.webroot));
});

gulp.task("watch:ts_root", function () {
	return gulp.watch(paths.ts, ["build:ts"]);
});

gulp.task("watch:ts_areas", function () {
	return gulp.watch(paths.areaTs, ["build:ts"]);
});

gulp.task("watch:ts", gulp.parallel("watch:ts_root", "watch:ts_areas"));

// Bundle JavaScript

gulp.task("bundle:itjakub", function () {
    return gulp.src([
            paths.webroot + "js/itjakub.js",
            paths.webroot + "js/itjakub.dataContracts.js",
            paths.webroot + "js/Plugins/itjakub.tools.js",
            paths.webroot + "js/Plugins/itjakub.components.js",
            paths.webroot + "js/Plugins/itjakub.eucookiepopup.js",
            paths.webroot + "js/ridics.form-validation.js",
            paths.webroot + "js/ridics.errorHandler.js",
            paths.webroot + "js/ridics.web-hub-api-client.js",
            paths.webroot + "js/ridics.list.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("itjakub.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "js"));
});

gulp.task("bundle:itjakub_plugins", function () {
    return gulp.src([
            paths.webroot + "js/Plugins/Progress/itjakub.plugins.progress.js",
            paths.webroot + "js/Plugins/itjakub.modul.inicializator.js",
            paths.webroot + "js/Plugins/itjakub.list.modul.inicializator.js",
            paths.webroot + "js/Plugins/itjakub.search.modul.inicializator.js",
            paths.webroot + "js/Plugins/Reader/itjakub.plugins.reader.js",
            paths.webroot + "js/Plugins/Bibliography/itjakub.plugins.bibliography.variableInterpreter.js",
            paths.webroot + "js/Plugins/Bibliography/itjakub.plugins.bibliography.factories.js",
            paths.webroot + "js/Plugins/Bibliography/itjakub.plugins.bibliography.configuration.js",
            paths.webroot + "js/Plugins/Bibliography/itjakub.plugins.bibliography.js",
            paths.webroot + "js/Plugins/Sort/itjakub.plugins.sort.js",
            paths.webroot + "js/Plugins/DropdownSelect/itjakub.plugins.dropdownselect.js",
            paths.webroot + "js/Plugins/DropdownSelect/itjakub.plugins.dropdownselect2.js",
            paths.webroot + "js/Plugins/RegExSearch/itjakub.plugins.regexsearch.js",
            paths.webroot + "js/Plugins/SearchBox/itjakub.plugins.searchbox.js",
            paths.webroot + "js/Plugins/SearchBox/itjakub.plugins.singlesearchbox.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("itjakub.plugins.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "js/Plugins"));
});

gulp.task("bundle:itjakub_keyboard", function () {
    return gulp.src([
            paths.webroot + "js/Plugins/Keyboard/itjakub.plugins.keyboard.js",
            paths.webroot + "js/Plugins/Keyboard/itjakub.plugins.keyboardComponent.js",
            paths.webroot + "js/Plugins/Keyboard/itjakub.plugins.keyboardManager.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("itjakub.keyboard.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "js/Plugins/Keyboard"));
});

gulp.task("bundle:itjakub_storage", function () {
    return gulp.src([
            paths.webroot + "js/Plugins/Storage/itjakub.plugins.storage.manager.js",
            paths.webroot + "js/Plugins/Storage/itjakub.plugins.storage.cookiestorage.js",
            paths.webroot + "js/Plugins/Storage/itjakub.plugins.storage.localstorage.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("itjakub.storage.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "js/Plugins/Storage"));
});

gulp.task("bundle:itjakub_favorite", function () {
    return gulp.src([
            paths.webroot + "js/Favorite/itjakub.favoriteApiClient.js",
            paths.webroot + "js/Favorite/itjakub.favoriteBook.js",
            paths.webroot + "js/Favorite/itjakub.favoriteManager.js",
            paths.webroot + "js/Favorite/itjakub.favoriteQuery.js",
            paths.webroot + "js/Favorite/itjakub.favoriteStar.js",
            paths.webroot + "js/Favorite/itjakub.newFavoriteDialog.js",
            paths.webroot + "js/Favorite/itjakub.newFavoriteNotification.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("itjakub.favorite.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "js/Favorite"));
});

gulp.task("bundlejs", gulp.parallel("bundle:itjakub", "bundle:itjakub_plugins", "bundle:itjakub_keyboard", "bundle:itjakub_storage", "bundle:itjakub_favorite"));


// Bundle JavaScript in Areas

gulp.task("bundle:itjakub_audiobooks", function () {
    return gulp.src([
            paths.webroot + "Areas/AudioBooks/js/itjakub.audiobooks.modul.inicializator.js",
            paths.webroot + "Areas/AudioBooks/js/itjakub.audiobooks.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("itjakub.audiobooks.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/AudioBooks/js"));
});

gulp.task("bundle:itjakub_bohemiantextbank", function () {
    return gulp.src([
            paths.webroot + "Areas/BohemianTextBank/js/itjakub.bohemiantextbank.modul.inicializator.js",
            paths.webroot + "Areas/BohemianTextBank/js/itjakub.bohemiantextbank.search.js",
            paths.webroot + "Areas/BohemianTextBank/js/itjakub.bohemiantextbank.list.js",
            paths.webroot + "Areas/BohemianTextBank/js/corpus-search-viewer/ridics.bohemiantextbank.*.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("itjakub.bohemiantextbank.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/BohemianTextBank/js"));
});

gulp.task("bundle:itjakub_cardfiles", function () {
    return gulp.src([
            paths.webroot + "Areas/CardFiles/js/itjakub.cardfiles.js",
            paths.webroot + "Areas/CardFiles/js/itjakub.cardfileManager.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("itjakub.cardfiles.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/CardFiles/js"));
});

gulp.task("bundle:itjakub_derivation", function () {
    return gulp.src([
            paths.webroot + "Areas/Derivation/js/itjakub.derivation.js",
            paths.webroot + "js/Plugins/Lemmatization/itjakub.lemmatization.shared.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("itjakub.derivation.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/Derivation/js"));
});

gulp.task("bundle:itjakub_dictionary_search", function () {
    return gulp.src([
            paths.webroot + "Areas/Dictionaries/js/itjakub.dictionaries.search.js",
            paths.webroot + "Areas/Dictionaries/js/itjakub.dictionariesViewer.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("itjakub.dictionaries.search.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/Dictionaries/js"));
});

gulp.task("bundle:itjakub_dictionary_headwords", function () {
    return gulp.src([
            paths.webroot + "Areas/Dictionaries/js/itjakub.dictionaries.headwords.js",
            paths.webroot + "Areas/Dictionaries/js/itjakub.dictionariesFavoriteHeadwords.js",
            paths.webroot + "Areas/Dictionaries/js/itjakub.dictionariesViewer.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("itjakub.dictionaries.headwords.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/Dictionaries/js"));
});

gulp.task("bundle:itjakub_lemmatization", function () {
    return gulp.src([
            paths.webroot + "Areas/Lemmatization/js/itjakub.lemmatization.js",
            paths.webroot + "Areas/Lemmatization/js/itjakub.lemmatization.list.js",
            paths.webroot + "js/Plugins/Lemmatization/itjakub.lemmatization.shared.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("itjakub.lemmatization.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/Lemmatization/js"));
});

gulp.task("bundle:itjakub_professionalliterature_list", function () {
    return gulp.src([
            paths.webroot + "Areas/ProfessionalLiterature/js/itjakub.professionalliterature.modul.inicializator.js",
            paths.webroot + "Areas/ProfessionalLiterature/js/itjakub.professionalliterature.list.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("itjakub.professionalliterature.list.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/ProfessionalLiterature/js"));
});

gulp.task("bundle:ridics_admin_project-editor", function () {
    return gulp.src([
            paths.webroot + "Areas/Admin/js/ridics.admin-api-client.js",
            paths.webroot + "Areas/Admin/js/ridics.project.client.js",
            paths.webroot + "Areas/Admin/js/ridics.project.module.js",
            paths.webroot + "Areas/Admin/js/ridics.project.resource.tabs.js",
            paths.webroot + "Areas/Admin/js/ridics.project.work.tabs.js",
            paths.webroot + "Areas/Admin/js/snapshot/ridics.snapshot.api-client.js",
            paths.webroot + "Areas/Admin/js/snapshot/ridics.snapshot.list.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("ridics.project.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/Admin/js"));
});

gulp.task("bundle:ridics_admin_text-editor", function () {
    return gulp.src([
            paths.webroot + "Areas/Admin/js/text-editor/ridics.project.text-editor*.js",
            "!" + paths.webroot + "Areas/Admin/js/text-editor/ridics.project.text-editor.bundle.js",
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("ridics.project.text-editor.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/Admin/js/text-editor"));
});

gulp.task("bundle:ridics_admin_page-image-viewer", function () {
    return gulp.src([
            paths.webroot + "Areas/Admin/js/page-image-viewer/ridics.project.page-image-viewer*.js",
            "!" + paths.webroot + "Areas/Admin/js/page-image-viewer/ridics.project.page-image-viewer.bundle.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("ridics.project.page-image-viewer.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/Admin/js/page-image-viewer"));
});

gulp.task("bundle:ridics_admin_page-list-editor", function () {
    return gulp.src([
            paths.webroot + "Areas/Admin/js/page-list-editor/ridics.project.page-list-editor*.js",
            "!" + paths.webroot + "Areas/Admin/js/page-list-editor/ridics.project.page-list-editor.bundle.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("ridics.project.page-list-editor.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/Admin/js/page-list-editor"));
});

gulp.task("bundle:ridics_admin_editors-common-base", function () {
    return gulp.src([
            paths.webroot + "Areas/Admin/js/editors-common-base/ridics.project.editors*.js",
            "!" + paths.webroot + "Areas/Admin/js/editors-common-base/ridics.project.editors.bundle.js",
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("ridics.project.editors.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/Admin/js/editors-common-base"));
});

gulp.task("bundle:ridics_admin_composition-key-table-editor", function () {
    return gulp.src([
            paths.webroot + "Areas/Admin/js/composition-key-table-editor/ridics.project.key-table-editor.table-base.js",
            paths.webroot + "Areas/Admin/js/composition-key-table-editor/ridics.project.key-table-editor!(table-base)*.js",
            "!" + paths.webroot + "Areas/Admin/js/composition-key-table-editor/ridics.project.key-table-editor.bundle.js.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("ridics.project.key-table-editor.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/Admin/js/composition-key-table-editor"));
});

gulp.task("bundle:ridics_admin-snapshot-editor", function () {
    return gulp.src([
            paths.webroot + "Areas/Admin/js/ridics.project.client.js",
            paths.webroot + "Areas/Admin/js/snapshot/ridics.snapshot.api-client.js",
            paths.webroot + "Areas/Admin/js/snapshot/ridics.snapshot.editor.js"
        ])
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("ridics.project.snapshot-editor.bundle.js"))
        //.pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.webroot + "Areas/Admin/js/snapshot"));
});

gulp.task("bundlejs_areas", gulp.parallel
(
    "bundle:itjakub_audiobooks",
    "bundle:itjakub_bohemiantextbank",
    "bundle:itjakub_cardfiles",
    "bundle:itjakub_derivation",
    "bundle:itjakub_dictionary_search",
    "bundle:itjakub_dictionary_headwords",
    "bundle:itjakub_lemmatization",
    "bundle:itjakub_professionalliterature_list",
    "bundle:ridics_admin_project-editor",
    "bundle:ridics_admin_editors-common-base",
    "bundle:ridics_admin_text-editor",
    "bundle:ridics_admin_page-image-viewer",
    "bundle:ridics_admin_page-list-editor",
    "bundle:ridics_admin_composition-key-table-editor",
    "bundle:ridics_admin-snapshot-editor"
));

gulp.task("build_bundle:ts", gulp.series("build:ts", gulp.parallel("bundlejs", "bundlejs_areas")));

//Download yarn dependencies

gulp.task("yarn-runtime", function () {
    return gulp.src(["./wwwroot/package.json", "./wwwroot/yarn.lock"])
        .pipe(yarn());
});

// Main build

gulp.task("default", gulp.parallel("build_bundle:ts", "build:less", "build:sass_lib"));

gulp.task("watch", gulp.parallel("watch:less", "watch:ts"));
