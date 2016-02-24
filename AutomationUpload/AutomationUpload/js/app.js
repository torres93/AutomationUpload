﻿var app = angular.module("AutomationUpload", ['ngRoute', 'ngMaterial', 'ngMessages']);
app.config(
	function ($routeProvider) {
	    $routeProvider.
		when('/', {
		    templateUrl: 'views/login.html',
            controller:"homeCtrl"
		}).
        when('/login', {
            templateUrl: 'views/login.html',
            controller: "homeCtrl"
        }).
        when('/admin',{
            templateUrl: 'views/admin.html',
            controller: "homeCtrl"
        })
     .otherwise({
         redirectTo: '/'
     });
	})

app.config(function ($mdThemingProvider) {
    $mdThemingProvider.theme('default')
      .primaryPalette('indigo')
      .accentPalette('pink')
      .warnPalette('red')
    ;
})


app.controller("homeCtrl", function () {

})