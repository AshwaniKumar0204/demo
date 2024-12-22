(function () {
    "use strict";

    angular.module("app")
        .service("loadDataService", loadDataService);

    loadDataService.$inject = [];

    function loadDataService() {
        this.getMaxDate = function () {
            var date = new Date();
            date.setDate(date.getDate() + 1);
            return date.toISOString().split('T')[0]
        }

        this.getInt = function (num) {
            if (num == null || num == undefined) {
                return 0;
            } else {
                return parseInt(num);
            }
        }

        this.getMinDate = function () {
            var date = new Date();
            return date.toISOString().split('T')[0]
        }

        this.getDateTime = function (InvoiceDate) {
            if (InvoiceDate == null || InvoiceDate == undefined)
                return null;
            var d = new Date(InvoiceDate);
            var dformat = [d.getMonth() + 1,
            d.getDate(),
            d.getFullYear()].join('/') + ' ' +
                [d.getHours(),
                d.getMinutes(),
                d.getSeconds()].join(':');
            return dformat;
        }

        this.getTime = function (InvoiceDate) {
            if (InvoiceDate == null || InvoiceDate == undefined)
                return null;
            var d = new Date(InvoiceDate);
            var dformat = [("0" + d.getHours()).slice(-2),
            ("0" + d.getMinutes()).slice(-2),
            ("0" + d.getSeconds()).slice(-2)].join(':');
            return dformat;
        }

        this.getDateDDMMYYYY = function (InvoiceDate) {
            if (InvoiceDate == null || InvoiceDate == undefined)
                return null;
            var d = new Date(InvoiceDate);
            var dateFromate = [
                ("0" + d.getDate()).slice(-2),
                ("0" + (d.getMonth() + 1)).slice(-2),
                d.getFullYear()
            ].join('/');
            return dateFromate;
        }

        this.getDateMMDDYYYY = function (InvoiceDate) {
            if (InvoiceDate == null || InvoiceDate == undefined)
                return null;
            var d = new Date(InvoiceDate);
            var dateFromate = [
                ("0" + (d.getMonth() + 1)).slice(-2),
                ("0" + d.getDate()).slice(-2),
                d.getFullYear()
            ].join('/');
            return dateFromate;
        }

        this.getUserDetail = function () {
            if (typeof (Storage) != undefined && typeof (Storage) != null) {
                if (localStorage.getItem('UserDetail') != null) {
                    return JSON.parse(localStorage.getItem('UserDetail'));
                } else {
                    return false;
                }
            } else
                return false;
        }
    }
}());