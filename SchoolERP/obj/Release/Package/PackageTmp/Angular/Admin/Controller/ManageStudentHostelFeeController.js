(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageStudentHostelFeeController', ManageStudentHostelFeeController);

    ManageStudentHostelFeeController.$inject = ['$scope', 'adminService', 'Factory', 'loadDataService'];

    function ManageStudentHostelFeeController($scope, adminService, Factory, loadDataService) {
        $scope.checkSysytemLogin();

        $scope.$watch('formHostel.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        //Hostel Hostel
        $scope.getHostelList = function () {
            $scope.dataLoading = true;
            adminService.getHostelList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.Hostels = response.data.Hostels;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.getHostelList();


        $scope.NewHostel = function () {
            $scope.IsSubmitted = false;
            $scope.StartDate = null;
            $scope.EndDate = null;
            $scope.StudentHostelId = 0;
            $("#modal_theme_primary").modal('show');
        }

        $scope.editFeePeriod = function (model) {
            $("#modal_theme_primary").modal('show');
        }

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

        $scope.getAdmissionDetail = function (AdmissionId, InputChange) {
            $scope.dataLoading = true;
            adminService.getAdmissionDetail(AdmissionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.Admission = response.data.AdmissionDetail;
                        $scope.AdmissionId = $scope.Admission.AdmissionId;
                        if (InputChange)
                            $scope.$broadcast('angucomplete-alt:changeInput', 'StudentAutoComplete', $scope.Admission.Student.AdmissionNo + " (" + $scope.Admission.Student.FullName + ")");
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
            $scope.getStudnetHostelList(AdmissionId);
        }

        $scope.getStudnetHostelList = function (AdmissionId) {
            adminService.getStudnetHostelList(AdmissionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StudnetHostelList = response.data.HostelList;
                    }
                    else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }

        $scope.checkSession = function () {
            var AdmisssionId = sessionStorage.getItem('admissionId');
            if (AdmisssionId > 0) {
                $scope.getAdmissionDetail(AdmisssionId, true);
                sessionStorage.removeItem('admissionId');
            } else {
                $scope.getSearchAdmissionList();
            }
        }
        $scope.checkSession();

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.SelectedAdmission = selected.originalObject;
                if ($scope.AdmissionId != $scope.SelectedAdmission.AdmissionId) {
                    $scope.getAdmissionDetail($scope.SelectedAdmission.AdmissionId, false);
                }
            };
        }

        $scope.getFeeHeadList = function () {
            $scope.dataLoading = true;
            adminService.getHostelFeeHeadList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.heads = response.data.FeeHeadLists;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.getFeeHeadList();


        $scope.editHostelDetails = function (model) {
            $("#modal_theme_primary").modal('show');
            var index = 0;
            for (var i = 0; i < $scope.Hostels.length; i++) {
                if ($scope.Hostels[i].HostelId == model.HostelId) {
                    index = i;
                    break;
                }
            }
            $scope.HostelTitle = $scope.Hostels[index];
            $scope.HostelId = $scope.Hostels[index].HostelId;
            if (model.EndDate != undefined && model.EndDate != null) {
                $scope.EndDate = new Date(model.EndDate);

            }
            $scope.StudentHostelId = model.StudentHostelId;
            $scope.StartDate = new Date(model.StartDate);
            $scope.Status = model.Status.toString();
        }

        $scope.SaveHostelDetails = function () {
            $scope.IsSubmitted = true;
            $scope.dataLoading = true;
            if ($scope.IsFormValid) {
                var item = {
                    StudentHostelId: $scope.StudentHostelId,
                    StudentId: $scope.Admission.StudentId,
                    HostelId: $scope.HostelTitle.HostelId,
                    StartDate: loadDataService.getDateTime($scope.StartDate),
                    EndDate: loadDataService.getDateTime($scope.EndDate),
                    Status: $scope.Status,
                }

                adminService.saveStudentHostel(item)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Student Hostel Details Saved Successfully!");
                            $scope.getStudnetHostelList($scope.Admission.AdmissionId);
                            $("#modal_theme_primary").modal('hide');

                            $scope.IsSubmitted = false;
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error(err.data.Message);
                        $scope.dataLoading = false;
                    })
            }
            else {
                toastr.error("Fields marked with * is required.", "Validation Error");
                $scope.dataLoading = false;
            }
        }
    }
})();
