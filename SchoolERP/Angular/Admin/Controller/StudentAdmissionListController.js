(function () {
    'use strict';

    angular
        .module('app')
        .controller('StudentAdmissionListController', StudentAdmissionListController);

    StudentAdmissionListController.$inject = ["$scope", "adminService", "ConstantData", "Factory","loadDataService"];

    function StudentAdmissionListController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };


        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(ConstantData.ClassStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                        var index = { ClassId: null, ClassName: '--Select Class--' };
                        $scope.ClassList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getClassList();

        $scope.getSectionList = function () {
            $scope.dataLoading = true;
            adminService.getSectionList(ConstantData.SectionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainSectionList = response.data.SectionList;
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

        $scope.changeClass = function () {
            $scope.SectionList = [];
            angular.forEach($scope.MainSectionList, function (section, key) {
                if (section.ClassId == $scope.Admission.ClassId) {
                    $scope.SectionList.push(section);
                }
            })
            var index = { SectionId: null, SectionName: '--Select Section--' };
            $scope.SectionList.unshift(index);
        }

        $scope.AdmissionTypeList = ConstantData.AdmissionType;
        $scope.AdmissionStatusList = ConstantData.AdmissionStatus;

        $scope.getSearchAdmissionList = function () {
            $scope.dataLoading = true;
            adminService.getSearchAdmissionList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AdmissionList = response.data.AdmissionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSearchAdmissionList();

        $scope.getStudentAdmissionList = function () {
            $scope.dataLoading = true;
            adminService.getStudentAdmissionList($scope.SelectedAdmission.StudentId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StudentAdmissionList = response.data.StudentAdmissionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.SelectedAdmission = selected.originalObject;
                if ($scope.AdmissionId != $scope.SelectedAdmission.AdmissionId) {
                    $scope.getStudentAdmissionList();
                    $scope.AdmissionId = $scope.SelectedAdmission.AdmissionId;
                }
            };
        }

        $scope.$watch('formAdmission.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.updateAdmission = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                $scope.dataLoading = true;
                //var AdmissionModel = {
                //    AdmissionId: $scope.Admission.AdmissionId,
                //    SectionId: $scope.Admission.SectionId,
                //    RollNo: $scope.Admission.RollNo,
                //}
                $scope.Admission.AdmissionDate = loadDataService.getDateTime($scope.Admission.AdmissionDate);
                adminService.updateAdmission($scope.Admission)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Student class detail Detail Updated Successfully.");
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getStudentAdmissionList();
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

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.formAdmission.$setPristine();
            $scope.formAdmission.$setUntouched();
        }

        $scope.editAdmission = function (AdmissionModel) {
            $scope.resetForm();
            $scope.Admission = AdmissionModel;
            $scope.Admission.AdmissionDate = new Date($scope.Admission.AdmissionDate);
            $scope.changeClass();
            $('#modal_Student_Charge').modal('show');
        }


    }
})();
