(function () {
    'use strict';

    angular
        .module('app')
        .controller('GenerateTimeTableTwoController', GenerateTimeTableTwoController);

    GenerateTimeTableTwoController.$inject = ['$scope', '$http'];

    function GenerateTimeTableTwoController($scope, $http) {
        $scope.AcademicSessionId = 3;
        $scope.ClassList = [];
        $scope.SelectedClassList = [];
        $scope.SectionIdList = [];
        $scope.SelectedSectionList = [];
        $http.get('/api/Data/GetSessionList').then((response) => {


            $scope.SessionList = response.data.SessionList;
            $scope.ClassList = response.data.ClassList;
            $scope.SectionList = response.data.SectionList;
            $scope.Conditions = response.data.Conditions;
            $scope.countLimit = 100;
            $scope.SectionId = null;
        }, () => {
            $scope.SessionList = [{ AcademicSessionId: 0, SessionName: "Error occured in Fetching SessionList" }];
        });

        $scope.filterSection = function () {
            if ($scope.ClassNo) {
                $scope.SectionId = "";
                $scope.filteredSectionList = $scope.SectionList.filter(x => x.ClassNo == $scope.ClassNo);
                $scope.SectionId = null;
            } else {
                $scope.SectionId = null;
            }
        }

        $scope.AddSection = function () {
            //$scope.ClassList.push($scope.ClassList.filter(x => x.ClassNo == $scope.ClassNo)[0]);

            if ($scope.SectionId) {
                $scope.SelectedSectionList.push($scope.SectionList.filter(x => x.SectionId == $scope.SectionId)[0]);
                $scope.SelectedSectionList = [...new Set($scope.SelectedSectionList)];
                $scope.SectionIdList.push($scope.SectionId);
                $scope.SectionIdList = [...new Set($scope.SectionIdList)];
            } else {
                $scope.SelectedSectionList.push(...$scope.filteredSectionList);
                $scope.SelectedSectionList = [...new Set($scope.SelectedSectionList)];
                $scope.filteredSectionList.forEach((section, index) => {
                    if (section.SectionId > 0) {
                        $scope.SectionIdList.push(section.SectionId);

                    }
                    $scope.SectionIdList = $scope.SectionIdList.filter(x => x > 0);
                });
                $scope.SectionIdList = [...new Set($scope.SectionIdList)];
            }
            $scope.SelectedClassList.push($scope.ClassNo);
        }

        $scope.GenerateTimeTable = function () {

            var requestData = {
                academicSessionId: $scope.AcademicSessionId,
                classList: $scope.SelectedClassList,
                sectionIdList: $scope.SectionIdList,
                countLimit: $scope.countLimit,
                Conditions: $scope.Conditions

            }

            $scope.DisplayMessage = "Creating TimeTable, process may take some time"
            $scope.ErrorMessage = null;
            $http({ method: "post", url: "/api/TimeTableNew/GenerateTimeTables", data: JSON.stringify(requestData), dataType: "JSON" })
                .then((response) => {
                    if (response.data.Message == "Success") {
                        $scope.GetSectionWiseList();
                        $scope.DisplayMessage = null;
                        $scope.ClassNo = null;
                        $scope.SectionId = null;
                        //$scope.SelectedSectionList = [];
                        //$scope.SelectedClassList = [];
                        $scope.countLimit = 100;
                    } else {
                        $scope.DisplayMessage = null;
                        $scope.ErrorMessage = response.data.Message;
                    }
                }, () => {
                    alert('Error');
                    $scope.DisplayMessage = null;
                });

        }

        $scope.ClearSelectedSectionList = function () {
            console.log("clicked");
            $scope.SelectedSectionList = [];
            $scope.SectionIdList = [];
            $scope.SelectedClassList = [];


        }
        //---------------------------------------------------------------------Sectionwise TimeTable List---------------------------------------------------------------------------------------------------
        $scope.GetSectionWiseList = function () {
            var requestData = {
                academicSessionId: $scope.AcademicSessionId,
                classList: $scope.SelectedClassList,
                sectionIdList: $scope.SectionIdList,
                countLimit: $scope.countLimit,
                Conditions: $scope.Conditions

            }
            $scope.DisplayMessage = "Fetching saved Timetables";
            $scope.ErrorMessage = null;
            $http({ method: "post", url: "/api/GetTimeTable/GetExistingTimeTables", data: JSON.stringify(requestData), dataType: "JSON" })
                .then((response) => {
                    if (response.data.Message == "Success") {
                        toastr.success("Timetable List fetched successfully!");
                        $scope.SectionWiseTimeTableList = response.data.SectionWiseTimeTableList;
                        $scope.DisplayMessage = null;
                        $scope.ClassNo = null;
                        $scope.SectionId = null;


                    } else {
                        $scope.DisplayMessage = null;
                        $scope.ErrorMessage = response.data.Message;
                    }
                }, () => {
                    toastr.error('Error');
                });
        }



        //--------------------------------------------------------------------------------Edit and Delete TimeTable---------------------------------------------------------------------------------------------------------
        $scope.DeleteSectionTimeTable = function (SectionId, SectionDetail) {
            $http.get('/api/Data/DeleteSectionTimeTable',{ params: { SectionId: SectionId } })
                .then((response) => {
                    if (response.data.Message == "Success") {
                        toastr.success("TimeTable for " + SectionDetail + " " + "deleted Successfully");
                        $scope.SectionWiseTimeTableList = $scope.SectionWiseTimeTableList.filter(x => x.SectionId != SectionId);
                    } else {
                        toastr.error("Error occured in deleting timetable")
                    }
                }, () => {
                    alert("Error Occured");})
          
        }

        $scope.EditSectionIdList = [];
        $scope.ToggleEdit = function(SectionId){
            if ($scope.EditSectionId==null ) {
                $scope.EditSectionId = SectionId;
            }else {
                if ($scope.EditSectionId == SectionId) {
                    $scope.EditSectionId = null;
                    $scope.EditSectionId = $scope.EditSectionIdList[0];
                    $scope.EditSectionIdList = $scope.EditSectionIdList.filter(x => x != $scope.EditSectionIdList[0]);
                } else {
                    if ($scope.EditSectionIdList.filter(x => x == SectionId).length == 0) {
                        $scope.EditSectionIdList.push(SectionId);
                    } else {
                        $scope.EditSectionIdList = $scope.EditSectionIdList.filter(x => x != SectionId);
                    }
                   
                }
            }
        }



        $scope.DeleteTimeTableSlot = function (SectionId, DayNo, PeriodNo) {
           
            $http.get('/api/Data/DeleteTimeTableSlot', { params: { SectionId: SectionId, DayNo:DayNo, PeriodNo:PeriodNo } })
                .then((response) => {
                    if (response.data.Message == "Success") {
                        //alert("Slot Deleted Succcessfully");
                        $scope.SectionWiseTimeTableList = $scope.SectionWiseTimeTableList.filter(x => x.SectionId != SectionId);
                        $scope.GetSectionWiseList();
                        $scope.EditSectionId = null;
                    } else {
                        toastr.error("Error occured in deleting slot");
                    }
                }, () => {
                    toastr.error("Error Occured");
                })
        }

        //--------------------------------------------------------------------------Check TimeTable against conditions-----------------------------------------------------------------------------------------
        $scope.CheckTimeTableForConditions = function (TimeTable) {
            const RequestData = {
                sectionTimeTable: TimeTable,
                AcademicSessionId: $scope.AcademicSessionId,
                conditions: $scope.Conditions
            };

            $http({ method: "post", url: "/api/TimeTableNew/CheckTimeTableForConditions", data: JSON.stringify(RequestData), dataType: "JSON" })
                .then((response) => {
                    if (response.data.Message == "Success") {
                        toastr.success("Test Successful");
                        $scope.CheckedConditions = response.data.CheckedConditions;
                        $scope.DisplayMessage = null;
                        $scope.ClassNo = null;
                        $scope.SectionId = null;
                        $scope.EditSectionId = TimeTable.SectionId;


                    } else {
                        $scope.DisplayMessage = null;
                        $scope.ErrorMessage = response.data.Message;
                    }
                }, () => {
                    toastr.error('Error');
                });

        }

        $scope.ClearCheckedCondition = function () {
            $scope.CheckedConditions = null;
        }


        //------------------------------------------------------Lab Report-----------------------------------------------------------------------------------
        $scope.FetchLabReport = function () {
            if ($scope.lab < 1 || $scope.lab==undefined) {
                toastr.warning("Please select a lab");
                $scope.LabReport = null;
                return;
            }
            $scope.LabMsg = "Fetching data, please wait.."
            $http.get('/api/Data/GetLabReport', { params: {  lab: $scope.lab, AcademicSessionId: $scope.AcademicSessionId } })
                .then((response) => {
                    if (response.data.Message == "Success") {
                        $scope.LabReport = response.data.LabReport;
                        toastr.success("Lab Report fetched Succcessfully");
                        $scope.LabMsg = null;
                        //$scope.SectionWiseTimeTableList = $scope.SectionWiseTimeTableList.filter(x => x.SectionId != SectionId);
                        //$scope.GetSectionWiseList();
                        //$scope.EditSectionId = null;
                    } else {
                        toastr.error("Error occured in deleting slot");
                        $scope.LabMsg = null;
                    }
                }, () => {
                    toastr.error("Error Occured");
                    $scope.LabMsg = null;
                })
        }
    };
})();
