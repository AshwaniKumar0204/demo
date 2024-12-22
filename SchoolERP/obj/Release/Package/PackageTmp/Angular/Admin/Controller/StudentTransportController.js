(function () {
    'use strict';

    angular
        .module('app')
        .controller('StudentTransportController', StudentTransportController);

    StudentTransportController.$inject = ['$scope', 'adminService', 'Factory', 'loadDataService'];

    function StudentTransportController($scope, adminService, Factory, loadDataService) {

        $scope.checkSysytemLogin();


        $scope.$watch('formTransport.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        //Transport group
        $scope.getGroupList = function () {
            $scope.dataLoading = true;
            adminService.getTransportGroupList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.groups = response.data.TransportGroups;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.getGroupList();

        //Bus
        $scope.GetBusList = function () {
            $scope.dataLoading = true;
            adminService.getBusList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.buses = response.data.Buses;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.GetBusList();

        $scope.NewTransport = function () {
            $scope.IsSubmitted = false;
            $scope.StartDate = "";
            $scope.PickupPoint = "";
            $scope.PickupTime = "";
            $scope.EndDate = "";
$scope.StudentTransportId = 0;
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
            $scope.GetTransportList(AdmissionId);
        }

        $scope.GetTransportList = function (AdmissionId) {
            adminService.getTransportList(AdmissionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.TransportList = response.data.TransportList;
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
            adminService.getTransportFeeHeadList()
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


        $scope.editTransportDetails = function (model) {
            $("#modal_theme_primary").modal('show');
            var index = 0;
            for (var i = 0; i < $scope.groups.length; i++) {
                if ($scope.groups[i].TransportGroupId == model.TransportGroupId) {
                    index = i;
                    break;
                }
            }
            $scope.TransportGroupTitle = $scope.groups[index];
            $scope.TransportGroupId = $scope.groups[index].TransportGroupId;

            for (var i = 0; i < $scope.buses.length; i++) {
                if ($scope.buses[i].BusId == model.BusId) {
                    index = i;
                    break;
                }
            }
            $scope.BusTitle = $scope.buses[index];
            $scope.BusId = $scope.buses[index].BusId;

            if (model.PickupTime != undefined && model.PickupTime != null) {
                
                // Split the time string into parts
                var timeParts = model.PickupTime.split(':');
                var hours = timeParts[0];
                var minutes = timeParts[1];

                // Combine hours and minutes to form the required "HH:MM" format
                //$scope.PickupTime = hours + ':' + minutes;
            }

            if (model.EndDate != undefined && model.EndDate != null) {
                $scope.EndDate = new Date(model.EndDate);

            }
            $scope.StudentTransportId = model.StudentTransportId;
            $scope.StartDate = new Date(model.StartDate);
            $scope.Status = model.Status.toString();
            $scope.PickupPoint = model.PickupPoint;
        }

        $scope.SaveTransportDetails = function () {
            $scope.IsSubmitted = true;

            var headList = [];

            $scope.dataLoading = true;



            if ($scope.IsFormValid) {

                var item = {
                    StudentTransportId: $scope.StudentTransportId,
                    StudentId: $scope.Admission.StudentId,
                    TransportGroupId: $scope.TransportGroupTitle.TransportGroupId,
                    BusId: $scope.BusTitle.BusId,
                    PickupPoint: $scope.PickupPoint,
                    PickupTime: loadDataService.getTime($scope.PickupTime),
                    StartDate:  loadDataService.getDateTime($scope.StartDate),
                    EndDate: loadDataService.getDateTime($scope.EndDate),
                    Status: $scope.Status.toString(),
                }

                adminService.saveTransortDetails(item)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Transport Details Saved Successfully!");
                            $scope.GetTransportList($scope.Admission.AdmissionId);
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
