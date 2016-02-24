var app = angular.module("AutomationUpload", ['ngRoute', 'ngMaterial', 'ngMessages','ngMdIcons']);
app.config(
	function ($routeProvider) {
	    $routeProvider.
		when('/', {
		    templateUrl: 'views/login.html',
            controller:"loginCtrl"
		}).
        when('/login', {
            templateUrl: 'views/login.html',
            controller: "loginCtrl"
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


app.controller("loginCtrl", ["$scope","$log",function($scope,$log)
{
    $scope.user = "";
    $scope.password = "";


}])

app.factory("$session", function () {
    return {
        //obtenemos una sesión //getter
        get: function (key) {
            return sessionStorage.getItem(key)
        },
        //creamos una sesión //setter
        set: function (key, val) {
            return sessionStorage.setItem(key, val)
        },
        //limpiamos una sesión
        clear: function (key) {
            return sessionStorage.removeItem(key)
        }
    }
})


app.factory('authUsers', function ($http, $location, $session, $mdDialog) {
    var cacheSession = function (username) {
        $session.set("userLogin", true);
        $session.set("username", username);
    }
    var unCacheSession = function () {
        $session.unset("userLogin");
        $session.unset("username");
    }

    return {
        //retornamos la función login de la factoria authUsers para loguearnos correctamente
        login: function (user) {
            var url = base_url + 'api/ingreso';
            return $http.post(url, user).success(function ($response) {

                console.log($response);

                if ($response.trim() === "successAd") {
                    cacheSession(user.username);
                    $location.path('/admin');
                }
                else if ($response.trim() === "successA") {
                    cacheSession(user.username);
                    $location.path("/alumno");
                }
                else if ($response.trim() === "successM") {
                    cacheSession(user.username);
                    $location.path("/maestro");
                }
                else if ($response.trim() === "successP") {
                    cacheSession(user.username);
                    $location.path("/padres");
                }
                else {
                    if ($response.trim() === "error_password") {
                        $mdDialog.show(
                              $mdDialog.alert()
                                .clickOutsideToClose(true)
                                .title('Aviso:')
                                .content('Contraseña Incorrecta')
                                .ok('Aceptar')
                            );
                        //alert("Contraseña Incorrecta");
                        //msg2=false;
                        //$scope.msg=true;
                    }
                    else {
                        //$scope.msg=false;
                        $mdDialog.show(
                           $mdDialog.alert()
                             .clickOutsideToClose(true)
                             .title('Aviso:')
                             .content('usuario Icorrecto')
                             .ok('Aceptar')
                         );

                    }
                }
            })
        },
        //función para cerrar la sesión del usuario
        logout: function () {
            return $http({
                url: "http://techsanher.esy.es"
            }).success(function () {
                //eliminamos la sesión de sessionStorage
                unCacheSession();
                $location.path("/");
            });
        },
        //función que comprueba si la sesión userLogin almacenada en sesionStorage existe
        isLoggedIn: function () {
            return sesionesControl.get("userLogin");
        }
    }
})
