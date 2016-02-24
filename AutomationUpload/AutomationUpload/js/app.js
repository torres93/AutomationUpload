var app = angular.module("AutomationUpload", ['ngRoute', 'ngMaterial', 'ngMessages']);
app.config(
	function ($routeProvider) {
	    $routeProvider.
		when('/', {
		    templateUrl: 'views/login.html',
            controller:""
		}).
        when('/login', {
            templateUrl: 'views/login.html',
        }).
        when('/admin',{
            templateUrl: 'views/admin.html',
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