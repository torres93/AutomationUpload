var app = angular.module("AutomationUpload", ['ngRoute', 'ngMaterial', 'ngMessages']);
app.config(
	function ($routeProvider) {
	    $routeProvider.
		when('/', {
		    templateUrl: 'views/login.html',
		    controller: "loginCtrl"
		}).
        when('/login', {
            templateUrl: 'views/login.html',
            controller: "loginCtrl"
        }).
        when('/admin', {
            templateUrl: 'views/admin.html',
            controller: "adminCtrl"
        }).
        when('/inicio', {
            templateUrl: 'views/inicio.html',
            controller:"userCtrl"
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
//factoria para guardar y eliminar sesiones con sessionStorage
app.factory("sesionesControl", function () {
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
        unset: function (key) {
            return sessionStorage.removeItem(key)
        }
    }
})

app.factory('authUsers', function ($http, $location, sesionesControl, $mdDialog) {
    var cacheSession = function (username) {
        sesionesControl.set("userLogin", true);
        sesionesControl.set("username", username);
    }
    var unCacheSession = function () {
        sesionesControl.unset("userLogin");
        sesionesControl.unset("username");
    }

    return {
        //retornamos la función login de la factoria authUsers para loguearnos correctamente
        login: function (user) {

            username = JSON.stringify({ username: user.username, password: user.password });
            return $http.post("wsApp.asmx/login", username).success(function ($response) {


                if ($response.d.trim() === "admin") {
                    cacheSession(user.username);
                    $location.path('/admin');
                }
                else if ($response.d.trim() === "userNormal") {
                    cacheSession(user.username);
                    $location.path("/inicio");
                }
                else {
                    $mdDialog.show(
                          $mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Aviso:')
                            .content('Datos Incorrectos')
                            .ok('Aceptar')
                        );
                }
            })
        },
        //función para cerrar la sesión del usuario
        logout: function () {
            return $http({
                url: "/"
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

app.controller("loginCtrl", function ($scope, $log, $location, authUsers, $mdDialog) {
    $scope.login = function (user) {
        if (user != undefined) {
            authUsers.login(user);
        }
        else {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Aviso')
                    .content('Ingresa tus datos')
                    .ok('Aceptar')
                )
        }

    }


})

app.controller("userCtrl", function ($scope, $http, authUsers, sesionesControl) {
    $http.post("wsApp.asmx/getEncuestas").success(function ($response) {
        console.log($response);
        $scope.encuestas = $response;
    });
    $scope.configVista = function (encuesta) {
       
        for (var i = 0; i < $scope.encuestas.length; i++) {
            if ($scope.encuestas[i].nombre == encuesta) {
                modelo = $scope.encuestas[i].id_modelo;
            }
        };
        modeloJson = JSON.stringify({ model: modelo });
        $http.post("wsApp.asmx/getCampos", modeloJson).success(function ($response) {
                $scope.campos = JSON.parse($response.d);    
        });
    }
    $scope.selected = [];
    $scope.toggle = function (campos, list) {
        var idx = list.indexOf(campos.nombre);
        if (idx > -1) list.splice(idx, 1);
        else list.push(campos.nombre);
    };
    $scope.exist = function (campos, list) {
        return list.indexOf(campos.nombre) > -1;
    }
})

app.controller("adminCtrl", function ($scope, $http, sesionesControl, authUsers, $location, $mdDialog) {
    $scope.addUser = "normal";
    $http.post("wsApp.asmx/getUsuarios").success(function ($response) {
        $scope.usuarios = $response;
    });
    $http.post("wsApp.asmx/getFuentes").success(function ($response) {
        $scope.fuentes = $response;
    });
    $scope.selected = [];
    $scope.editarUsuario = function (usr) {
        $scope.userEdit = usr;
        console.log(usr.fuentes);
        $scope.selected = usr.fuentes;

    }

    
    $scope.toggle = function (fuentes, list) {
        var aux = "";
        var cont = 0;
        if (list.length > 0) {
            for (var i = 0; i < list.length; i++) {
                if (list[i].nombre == fuentes.nombre) {
                    console.log(list[i].nombre + " " + fuentes.nombre);
                    aux = "existe";
                    cont = i;
                    
                }
            }
            console.log(aux);
            console.log(cont);
            if (aux == "existe") {
                console.log(cont);
                list.splice(cont, 1);
            }
            else {
                list.push(fuentes);
            }
        } else {
            list.push(fuentes);
        }
    
 
    };
    $scope.exist = function (fuentes, list) {
        var str = JSON.stringify(list);
        var f = JSON.stringify(fuentes);
        return str.indexOf(f) > -1;
    }

    $scope.editUser = function (userEdit) {
        console.log(userEdit);
        userEdit.fuentes = $scope.selected;

        $http.post("wsApp.asmx/updateUsuario",userEdit).success(function ($response) {
            $mdDialog.show(
                $mdDialog.alert()
                    .clickOutsideToClose(true)
                    .title('Aviso')
                    .content('Cambios guardados')
                    .ok('Aceptar')
            )
            $scope.userEdit = "";
            $scope.selected = [];
        });

    }
    $scope.creaarUsuario = function (userN) {
        userN.fuentes = $scope.selected;
        $http.post("wsApp.asmx/createUsuario",userN).success(function ($response) {
                $mdDialog.show(
                    $mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Aviso')
                        .content('Nuevo usuario creado')
                        .ok('Aceptar')
                        
                )
            $scope.selected = [];
            $scope.userN = "";
            $scope.addUser = "normal";
            
            $http.post("wsApp.asmx/getUsuarios").success(function ($response) {
                $scope.usuarios = $response;
            });
        })
    }

})



//mientras corre la aplicación, comprobamos si el usuario tiene acceso a la ruta a la que está accediendo
//como vemos inyectamos authUsers

app.run(function ($rootScope, $location, authUsers) {

    //creamos un array con las rutas que queremos controlar
    var rutasPrivadas = ["/admin"];
    //al cambiar de rutas
    $rootScope.$on('$routeChangeStart', function () {
        //si en el array rutasPrivadas existe $location.path(), locationPath en el login
        //es /login, en la home /home etc, o el usuario no ha iniciado sesión, lo volvemos 
        //a dejar en el formulario de login
        if (in_array($location.path(), rutasPrivadas) && !authUsers.isLoggedIn()) {
            $location.path("/login");
        }
        //en el caso de que intente acceder al login y ya haya iniciado sesión lo mandamos a la home
        if (($location.path() === '/login') && authUsers.isLoggedIn()) {
            $location.path("/inicio");
        }
    });
});

//función in_array que usamos para comprobar si el usuario
//tiene permisos para estar en la ruta actual
function in_array(needle, haystack, argStrict) {
    var key = '',
    strict = !!argStrict;

    if (strict) {
        for (key in haystack) {
            if (haystack[key] === needle) {
                return true;
            }
        }
    } else {
        for (key in haystack) {
            if (haystack[key] == needle) {
                return true;
            }
        }
    }
    return false;
}
