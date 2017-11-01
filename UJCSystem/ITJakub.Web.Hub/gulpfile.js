/// <binding AfterBuild='default' Clean='clean' ProjectOpened='watch:less' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require("gulp"),
  rimraf = require("rimraf"),
  concat = require("gulp-concat"),
  cssmin = require("gulp-cssmin"),
  less = require("gulp-less"),
  watch = require("gulp-watch"),
  sourcemaps = require('gulp-sourcemaps'),
  uglify = require("gulp-uglify"),
  ts = require("gulp-typescript");
var tsProject = ts.createProject("tsconfig.json");

var paths = {
    webroot: "./wwwroot/"
};

paths.less = paths.webroot + "css/**/*.less";
paths.css = paths.webroot + "css/**/*.css";
paths.js = paths.webroot + "js/**/*.js";
paths.jsMap = paths.webroot + "js/**/*.js.map";
paths.areaLess = paths.webroot + "Areas/*/css/**/*.less";
paths.areaCss = paths.webroot + "Areas/*/css/**/*.css";
paths.areaJs = paths.webroot + "Areas/*/js/**/*.js";
paths.areaJsMap = paths.webroot + "Areas/*/js/**/*.js.map";


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

gulp.task("clean", ["clean:css", "clean:css_areas"/*, "clean:js", "clean:js_areas", "clean:jsmap", "clean:jsmap_areas"*/]);


// Less build

gulp.task("build:less_root", function () {
    return gulp.src(paths.less)
        .pipe(less({
            relativeUrls: true
        }))
        .pipe(gulp.dest(paths.webroot + "css"));
});

gulp.task("build:less_areas", function () {
    return gulp.src(paths.areaLess)
        .pipe(less({
            relativeUrls: true
        }))
        .pipe(gulp.dest(paths.webroot + "Areas"));
});

gulp.task("build:less", ["build:less_root", "build:less_areas"]);

gulp.task("watch:less_root", function() {
    return gulp.watch(paths.less, ["build:less_root"]);
});

gulp.task("watch:less_areas", function () {
    return gulp.watch(paths.areaLess, ["build:less_areas"]);
});

gulp.task("watch:less", ["watch:less_root", "watch:less_areas"]);


// TypeScript build
gulp.task("build:ts", function () {
    var tsResult = tsProject.src()
        .pipe(tsProject());

    return tsResult.js.pipe(gulp.dest(paths.webroot));
});


// Bundle JavaScript

gulp.task("bundle:itjakub", function () {
    return gulp.src([
            paths.webroot + "js/itjakub.js",
            paths.webroot + "js/itjakub.dataContracts.js",
            paths.webroot + "js/Plugins/itjakub.tools.js",
            paths.webroot + "js/Plugins/itjakub.components.js",
            paths.webroot + "js/Plugins/itjakub.eucookiepopup.js"
        ])
        .pipe(sourcemaps.init())
        .pipe(concat("itjakub.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "js/bundles"));
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
        .pipe(sourcemaps.init())
        .pipe(concat("itjakub.plugins.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "js/bundles"));
});

gulp.task("bundle:itjakub_keyboard", function () {
    return gulp.src([
            paths.webroot + "js/Plugins/Keyboard/itjakub.plugins.keyboard.js",
            paths.webroot + "js/Plugins/Keyboard/itjakub.plugins.keyboardComponent.js",
            paths.webroot + "js/Plugins/Keyboard/itjakub.plugins.keyboardManager.js"
        ])
        .pipe(sourcemaps.init())
        .pipe(concat("itjakub.keyboard.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "js/bundles"));
});

gulp.task("bundle:itjakub_storage", function () {
    return gulp.src([
            paths.webroot + "js/Plugins/Storage/itjakub.plugins.storage.manager.js",
            paths.webroot + "js/Plugins/Storage/itjakub.plugins.storage.cookiestorage.js",
            paths.webroot + "js/Plugins/Storage/itjakub.plugins.storage.localstorage.js"
        ])
        .pipe(sourcemaps.init())
        .pipe(concat("itjakub.storage.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "js/bundles"));
});

gulp.task("bundle:itjakub_favorite", function () {
    return gulp.src([
            paths.webroot + "js/Favorite/itjakub.favoriteBook.js",
            paths.webroot + "js/Favorite/itjakub.favoriteManager.js",
            paths.webroot + "js/Favorite/itjakub.favoriteQuery.js",
            paths.webroot + "js/Favorite/itjakub.favoriteStar.js",
            paths.webroot + "js/Favorite/itjakub.newFavoriteDialog.js",
            paths.webroot + "js/Favorite/itjakub.newFavoriteNotification.js"
        ])
        .pipe(sourcemaps.init())
        .pipe(concat("itjakub.favorite.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "js/bundles"));
});

gulp.task("bundlejs", ["bundle:itjakub", "bundle:itjakub_plugins", "bundle:itjakub_keyboard", "bundle:itjakub_storage", "bundle:itjakub_favorite"]);


// Bundle JavaScript in Areas

gulp.task("bundle:itjakub_audiobooks", function () {
    return gulp.src([
            paths.webroot + "Areas/AudioBooks/js/itjakub.audiobooks.modul.inicializator.js",
            paths.webroot + "Areas/AudioBooks/js/itjakub.audiobooks.js"
        ])
        .pipe(sourcemaps.init())
        .pipe(concat("itjakub.audiobooks.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "Areas/AudioBooks/js"));
});

gulp.task("bundle:itjakub_bohemiantextbank", function () {
    return gulp.src([
            paths.webroot + "Areas/BohemianTextBank/js/itjakub.bohemiantextbank.modul.inicializator.js",
            paths.webroot + "Areas/BohemianTextBank/js/itjakub.bohemiantextbank.search.js",
            paths.webroot + "Areas/BohemianTextBank/js/itjakub.bohemiantextbank.list.js"
        ])
        .pipe(sourcemaps.init())
        .pipe(concat("itjakub.bohemiantextbank.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "Areas/BohemianTextBank/js"));
});

gulp.task("bundle:itjakub_cardfiles", function () {
    return gulp.src([
            paths.webroot + "Areas/CardFiles/js/itjakub.cardfiles.js",
            paths.webroot + "Areas/CardFiles/js/itjakub.cardfileManager.js"
        ])
        .pipe(sourcemaps.init())
        .pipe(concat("itjakub.cardfiles.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "Areas/CardFiles/js"));
});

gulp.task("bundle:itjakub_derivation", function () {
    return gulp.src([
            paths.webroot + "Areas/Derivation/js/itjakub.derivation.js",
            paths.webroot + "js/Plugins/Lemmatization/itjakub.lemmatization.shared.js"
        ])
        .pipe(sourcemaps.init())
        .pipe(concat("itjakub.derivation.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "Areas/Derivation/js"));
});

gulp.task("bundle:itjakub_dictionary_search", function () {
    return gulp.src([
            paths.webroot + "Areas/Dictionaries/js/itjakub.dictionaries.search.js",
            paths.webroot + "Areas/Dictionaries/js/itjakub.dictionariesViewer.js"
        ])
        .pipe(sourcemaps.init())
        .pipe(concat("itjakub.dictionaries.search.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "Areas/Dictionaries/js"));
});

gulp.task("bundle:itjakub_dictionary_headwords", function () {
    return gulp.src([
            paths.webroot + "Areas/Dictionaries/js/itjakub.dictionaries.headwords.js",
            paths.webroot + "Areas/Dictionaries/js/itjakub.dictionariesFavoriteHeadwords.js",
            paths.webroot + "Areas/Dictionaries/js/itjakub.dictionariesViewer.js"
        ])
        .pipe(sourcemaps.init())
        .pipe(concat("itjakub.dictionaries.headwords.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "Areas/Dictionaries/js"));
});

gulp.task("bundle:itjakub_lemmatization", function () {
    return gulp.src([
            paths.webroot + "Areas/Lemmatization/js/itjakub.lemmatization.js",
            paths.webroot + "Areas/Lemmatization/js/itjakub.lemmatization.list.js",
            paths.webroot + "js/Plugins/Lemmatization/itjakub.lemmatization.shared.js"
        ])
        .pipe(sourcemaps.init())
        .pipe(concat("itjakub.lemmatization.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "Areas/Lemmatization/js"));
});

gulp.task("bundle:itjakub_professionalliterature_list", function () {
    return gulp.src([
            paths.webroot + "Areas/ProfessionalLiterature/js/itjakub.professionalliterature.modul.inicializator.js",
            paths.webroot + "Areas/ProfessionalLiterature/js/itjakub.professionalliterature.list.js"
        ])
        .pipe(sourcemaps.init())
        .pipe(concat("itjakub.professionalliterature.list.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "Areas/ProfessionalLiterature/js"));
});

gulp.task("bundle:ridics_admin_text-editor", function () {
    return gulp.src([
            paths.webroot + "Areas/Admin/js/text-editor/ridics.project.text-editor*.js"
        ])
        .pipe(sourcemaps.init())
        .pipe(concat("ridics.project.text-editor.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "Areas/Admin/js"));
});

gulp.task("bundle:ridics_admin_page-image-viewer", function () {
    return gulp.src([
        paths.webroot + "Areas/Admin/js/page-image-viewer/ridics.project.page-image-viewer*.js"
        ])
        .pipe(sourcemaps.init())
        .pipe(concat("ridics.project.page-image-viewer.bundle.js"))
        .pipe(sourcemaps.write())
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "Areas/Admin/js"));
});

gulp.task("bundlejs_areas",
[
    "bundle:itjakub_audiobooks",
    "bundle:itjakub_bohemiantextbank",
    "bundle:itjakub_cardfiles",
    "bundle:itjakub_derivation",
    "bundle:itjakub_dictionary_search",
    "bundle:itjakub_dictionary_headwords",
    "bundle:itjakub_lemmatization",
    "bundle:itjakub_professionalliterature_list",
    "bundle:ridics_admin_text-editor",
    "bundle:ridics_admin_page-image-viewer"
]);


// Main build

gulp.task("default", ["build:less", "bundlejs", "bundlejs_areas"]);