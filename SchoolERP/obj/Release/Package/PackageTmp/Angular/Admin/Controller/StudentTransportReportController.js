(function () {
    'use strict';

    angular
        .module('app')
        .controller('StudentTransportReportController', StudentTransportReportController);

    StudentTransportReportController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData'];

    function StudentTransportReportController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.StateList = [];
        $scope.IsSubmitted = false;
        $scope.FilterOptions = [{ Key: 1, Value: 'All Students' }, { Key: 2, Value: 'Pickup Point Wise' }, { Key: 3, Value: 'Bus/Van Wise' }];

        $scope.$watch('formReport.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.getAcademicSessionList = function () {
            if ($scope.ClassId == 0) {
                $scope.AcademicSessionList = [];
                $scope.AcademicSessionId = null;
                return;
            }
            var obj = {
                ClassId: $scope.ClassId
            }
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getAcademicSessionList();

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(ConstantData.ClassStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
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
                        $scope.changeClass();
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
                if (section.ClassId == $scope.ClassId) {
                    $scope.SectionList.push(section);
                }
            })
        }

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

        $scope.SubmitReport = function () {
            $scope.IsSubmitted = true;
            var groupId = 0;
            if ($scope.TransportGroupTitle != null) {
                groupId = $scope.TransportGroupTitle.TransportGroupId;
            }

            if ($scope.IsFormValid) {
                var classId = 0, sectionId = 0;
                if ($scope.ClassId)
                    classId = $scope.ClassId;
                if ($scope.SectionId)
                    sectionId = $scope.SectionId;
                if ($scope.ReportFor == 1 && $scope.AcademicSessionId != undefined) {
                    window.open("/print/TransportReportForAllStudent?sessionId=" + $scope.AcademicSessionId + "&sectionId="+ sectionId+'&classId=' + classId);
                }
                else if ($scope.ReportFor == 2) {
                    var transportGroupId = 0;
                    if ($scope.transportGroupId > 0)
                        transportGroupId = $scope.transportGroupId;
                    window.open("/print/TrasnportReportGroupWise?sessionId=" + $scope.AcademicSessionId + "&transportGroupId=" + transportGroupId + "&sectionId=" + sectionId + '&classId=' + classId);
                } else if ($scope.ReportFor == 3) {
                    var busId = 0;
                    if ($scope.busId > 0)
                        busId = $scope.busId;
                    window.open("/print/TransportReportBusWise?sessionId=" + $scope.AcademicSessionId + "&busId=" + busId + "&sectionId=" + sectionId + '&classId=' + classId);
                }
            }
            else {
                toastr.error("Fields marked with * is required.", "Validation Error");
            }
        }
    }
}());