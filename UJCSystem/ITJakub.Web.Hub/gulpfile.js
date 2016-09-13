/// <binding BeforeBuild='build' Clean='clean' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require("gulp"),
  rimraf = require("rimraf"),
  concat = require("gulp-concat"),
  cssmin = require("gulp-cssmin"),
  less = require("gulp-less"),
  uglify = require("gulp-uglify");

var paths = {
    webroot: "./wwwroot/"
};

paths.less = paths.webroot + "css/**/*.less";
paths.css = paths.webroot + "css/**/*.css";
paths.js = paths.webroot + "js/**/*.js";
paths.areaLess = paths.webroot + "Areas/*/css/**/*.less";
paths.areaCss = paths.webroot + "Areas/*/css/**/*.css";
paths.areaJs = paths.webroot + "Areas/*/js/**/*.js";


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

gulp.task("clean", ["clean:css", "clean:css_areas", "clean:js", "clean:js_areas"]);


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


// Bundle JavaScript

gulp.task("bundle:itjakub", function () {
    return gulp.src([
            paths.webroot + "js/itjakub.js",
            paths.webroot + "js/Plugins/itjakub.eucookiepopup.js"
        ])
        .pipe(concat("itjakub.bundle.js"))
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "js/bundles"));
});

gulp.task("bundle:itjakub_plugins", function () {
    return gulp.src([
            paths.webroot + "js/itjakub.js",
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
            paths.webroot + "js/Plugins/itjakub.plugins.pagination.js",
            paths.webroot + "js/Plugins/SearchBox/itjakub.plugins.searchbox.js",
            paths.webroot + "js/Plugins/SearchBox/itjakub.plugins.singlesearchbox.js"
        ])
        .pipe(concat("itjakub.plugins.bundle.js"))
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "js/bundles"));
});

gulp.task("bundle:itjakub_keyboard", function () {
    return gulp.src([
            paths.webroot + "js/Plugins/Keyboard/itjakub.plugins.keyboard.js",
            paths.webroot + "js/Plugins/Keyboard/itjakub.plugins.keyboardComponent.js",
            paths.webroot + "js/Plugins/Keyboard/itjakub.plugins.keyboardManager.js"
        ])
        .pipe(concat("itjakub.keyboard.bundle.js"))
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "js/bundles"));
});

gulp.task("bundle:itjakub_storage", function () {
    return gulp.src([
            paths.webroot + "js/Plugins/Storage/itjakub.plugins.storage.manager.js",
            paths.webroot + "js/Plugins/Storage/itjakub.plugins.storage.cookiestorage.js",
            paths.webroot + "js/Plugins/Storage/itjakub.plugins.storage.localstorage.js"
        ])
        .pipe(concat("itjakub.storage.bundle.js"))
        //.pipe(uglify())
        .pipe(gulp.dest(paths.webroot + "js/bundles"));
});

gulp.task("bundlejs", ["bundle:itjakub", "bundle:itjakub_plugins", "bundle:itjakub_keyboard", "bundle:itjakub_storage"]);


// Main build

gulp.task("build", ["build:less", "bundlejs"]);