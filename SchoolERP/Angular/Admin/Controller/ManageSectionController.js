(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageSectionController', ManageSectionController);

    ManageSectionController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageSectionController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.SectionStatusList = ConstantData.SectionStatus;
        $scope.SectionId = null;


        $scope.Section = {};
        $scope.getSectionList = function () {
            if ($scope.ClassId == null) {
                $scope.SectionList = [];
                return;
            }
            $scope.dataLoading = true;
            adminService.getSectionList2(0, $scope.ClassId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SectionList = response.data.SectionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSectionList();

        $scope.ClassId = null;
        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                });
        }, function (err) {
            toastr.error("Some error occured while fetching data");
            $scope.dataLoading = false;
        }
        $scope.getClassList();

        $scope.$watch('formSection.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.SaveSection = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var SectionModel = {
                    SectionId: $scope.Section.SectionId,
                    ClassId: $scope.Section.ClassId,
                    SectionStatus: $scope.Section.SectionStatus,
                    SectionName: $scope.Section.SectionName,
                }
                console.log(SectionModel)
                $scope.dataLoading = true;
                adminService.saveSection(SectionModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (SectionModel.SectionId > 0)
                                toastr.success("Section Updated Successfully.");
                            else
                                toastr.success("New Section Created Successfully.");
                            $scope.SectionList = response.data.SectionList;
                            $('#modal_Student_Charge').modal('hide');
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
        }

        $scope.Section = {};
        $scope.Section.SectionId = null;
        $scope.Section.ClassId = null;
        $scope.Section.SectionName = null;
        $scope.Section.SectionStatus = null;

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.Section.SectionId = null;
            $scope.Section.ClassId = $scope.ClassId;
            $scope.Section.SectionName = null;
            $scope.Section.SectionStatus = null;
            $scope.formSection.$setPristine();
            $scope.formSection.$setUntouched();
        }

        $scope.newSection = function () {
            if ($scope.ClassId > 0) {
                $scope.resetForm();
                $('#modal_Student_Charge').modal('show');
            } else {
                toastr.error("Class is required !!");
            }
        }

        $scope.editSection = function (SectionModel) {
            $scope.resetForm();
            $scope.Section.SectionId = SectionModel.SectionId;
            $scope.Section.ClassId = SectionModel.ClassId;
            $scope.Section.SectionName = SectionModel.SectionName;
            $scope.Section.SectionStatus = SectionModel.SectionStatus;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (SectionId) {
            $scope.SectionId = SectionId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteSection = function () {
            $scope.dataLoading = true;
            adminService.deleteSection($scope.SectionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Section deleted successfully.", "Deleted Successfully.")
                        $scope.SectionList = response.data.SectionList;
                        $scope.SectionId = null;
                        $('#modal_confirmation').modal('hide');
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
    }

})();

