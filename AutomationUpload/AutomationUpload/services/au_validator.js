

app.service("$au_validator", function ($http) {

    this.fnValidateLength = function (modelo) {
        alert("");
        var m = { id_modelo: modelo }
        $http.post("wsApp.asmx/getWorkTable", m).success(function ($response) {
            console.log($response.d);
        });
    }

    this.InsertValues = function()
    {
        $http.post("wsApp.asmx/putWorkTable", m).success(function ($response) {
            console.log($response.d);
        });
    }
});