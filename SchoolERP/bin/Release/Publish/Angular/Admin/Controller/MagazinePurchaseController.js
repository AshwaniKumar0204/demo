(function () {
    'use strict';

    angular
        .module('app')
        .controller('MagazinePurchaseController', MagazinePurchaseController);

    MagazinePurchaseController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function MagazinePurchaseController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.MagazinePurchase = {};

        $scope.getMagazineList = function () {
            $scope.dataLoading = true;
            adminService.getMagazineList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MagazineList = response.data.MagazineList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getMagazineList();

        $scope.getMagazinePurchaseList = function () {
            $scope.TotalAmount = 0;
            if ($scope.MagazineId == null || $scope.MagazineId == undefined) {
                $scope.MagazinePurchaseList = [];
                return;
            }
            $scope.dataLoading = true;
            adminService.getMagazinePurchaseList($scope.MagazineId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MagazinePurchaseList = response.data.MagazinePurchaseList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getMagazinePurchaseList();

        $scope.getMediumList = function () {
            $scope.dataLoading = true;
            adminService.getMediumList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MediumList = response.data.MediumList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getMediumList();

        $scope.$watch('formMagazinePurchase.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveMagazinePurchase = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var MagazinePurchaseModel = {
                    MagazinePurchaseId: $scope.MagazinePurchase.MagazinePurchaseId,
                    Medium: $scope.MagazinePurchase.Medium,
                    Source: $scope.MagazinePurchase.Source,
                    Amount: $scope.MagazinePurchase.Amount,
                    MagazineId: $scope.MagazinePurchase.MagazineId,
                    Remarks: $scope.MagazinePurchase.Remarks,
                    PublishDate: loadDataService.getDateTime($scope.MagazinePurchase.PublishDate),
                    ReceiveDate: loadDataService.getDateTime($scope.MagazinePurchase.ReceiveDate),
                }

                $scope.dataLoading = true;
                adminService.saveMagazinePurchase(MagazinePurchaseModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (MagazinePurchaseModel.MagazinePurchaseId > 0)
                                toastr.success("Magazine Purchase Updated Successfully.");
                            else
                                toastr.success("New Magazine Purchase Created Successfully.");
                            $scope.resetForm();
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getMagazinePurchaseList();
                        } else {
                            toastr.error(response.data.Message);
                            $scope.dataLoading = false;
                        }
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
        }

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.MagazinePurchase = {};
            $scope.MagazinePurchase.PublishDate = new Date();
            $scope.MagazinePurchase.ReceiveDate = new Date();
            $scope.formMagazinePurchase.$setPristine();
            $scope.formMagazinePurchase.$setUntouched();
        }

        $scope.newMagazinePurchase = function () {
            $scope.resetForm();
            $scope.MagazinePurchase.MagazineId = $scope.MagazineId;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editMagazinePurchase = function (MagazinePurchaseModel) {
            $scope.resetForm();
            $scope.MagazinePurchase = MagazinePurchaseModel;
            $scope.MagazinePurchase.PublishDate = new Date(MagazinePurchaseModel.PublishDate);
            $scope.MagazinePurchase.ReceiveDate = new Date(MagazinePurchaseModel.ReceiveDate);
            $scope.MagazinePurchase.Amount = parseInt(MagazinePurchaseModel.Amount);
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (MagazinePurchaseId) {
            $scope.MagazinePurchaseId = MagazinePurchaseId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteMagazinePurchase = function () {
            $scope.dataLoading = true;
            adminService.deleteMagazinePurchase($scope.MagazinePurchaseId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Magazine Purchase deleted successfully.", "Deleted Successfully.")
                        $scope.MagazinePurchaseId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.getMagazinePurchaseList();
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.tablesToExcel = function (tables, wsnames, wbname, appname) {
            var initailPage = $scope.pageSize;
            $scope.pageSize = $scope.sizeOptions[$scope.sizeOptions - 1];
            $scope.IsExporting = true;
            setTimeout(function () {
                tablesToExcel(tables, wsnames, wbname, appname);
            }, 500)
            setTimeout(function () {
                $scope.IsExporting = false;
                $scope.pageSize = initailPage;
                $scope.$apply();
            }, 1000)
        }
    }
})();
