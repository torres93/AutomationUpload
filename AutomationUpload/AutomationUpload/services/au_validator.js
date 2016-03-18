

app.service("$au_validator", function ($http) {
    this.VdE = false;


    this.fnValidateLength = function (modelo,data) {
        var m = { id_modelo: modelo }
       return $http.post("wsApp.asmx/getWorkTable", m).success(function ($response) {                    
        });
    }

    this.InsertValues = function()
    {
        $http.post("wsApp.asmx/putWorkTable", m).success(function ($response) {
            console.log($response.d);
        });
    }
});