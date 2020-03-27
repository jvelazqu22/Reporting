using System.Collections.Generic;
using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.PTATravelersDetail
{
    public static class ExportFieldHandler
    {
        public static List<string> GetExportFields(bool includeLostSavings, bool includeAuthComm, UserBreaks userBreaks, ReportGlobals globals)
        {
            var fields = new List<string>();

            if (globals.Agency.EqualsIgnoreCase("AXI"))
            {

                fields.Add("Reckey as Record_Key");
                fields.Add("TravAuthNo as Travel_Authorization_Number");
                fields.Add("RowNum as Row_Number");
                fields.Add("Acct as Account_Number");
                fields.Add("AcctDesc as Account_Name");
                

                if (userBreaks.UserBreak1)
                    fields.Add("break1 as " + globals.User.Break1Name.Replace("Break1","Break_1"));
                if (userBreaks.UserBreak2)
                    fields.Add("break2 as " + globals.User.Break2Name.Replace("Break2", "Break_2"));
                if (userBreaks.UserBreak3)
                    fields.Add("break3 as " + globals.User.Break3Name.Replace("Break3", "Break_3"));

                fields.Add("PassLast as Passenger_Last_Name");
                fields.Add("PassFrst as Passenger_First_Name");
                fields.Add("RecLoc as Record_Locator");
                fields.Add("bookedgmt as Booked_Date");
                fields.Add("TravReason as Reason_for_Travel");
                fields.Add("CliAuthNbr as Client_Input_Auth_Number");

                if (includeAuthComm) fields.Add("authcomm as authorization_comments");

                fields.Add("StatusDate as Notification_or_Authorization_Action_Status_Date");
                fields.Add("StatusTime as Notification_or_Authorization_Action_Status_Time");
                fields.Add("StatusDesc as Notification_or_Authorization_Action_Description");

                fields.Add("OOPReason1 as Out_of_Policy_Reason_1");
                fields.Add("OOPReason2 as Out_of_Policy_Reason_2");
                fields.Add("OOPReason3 as Out_of_Policy_Reason_3");
                fields.Add("OOPReason4 as Out_of_Policy_Reason_4");
                fields.Add("OOPReason5 as Out_of_Policy_Reason_5");

                fields.Add("AuthEmail1 as Authorizer_1_Email");
                fields.Add("AuthEmail2 as Authorizer_2_Email");
                fields.Add("AuthEmail3 as Authorizer_3_Email");
                fields.Add("AuthEmail4 as Authorizer_4_Email");
                fields.Add("AuthEmail5 as Authorizer_5_Email");

                fields.Add("Authorizr1 as Authorizer_Who_Actioned_1");
                fields.Add("Authorizr2 as Authorizer_Who_Actioned_2");
                fields.Add("Authorizr3 as Authorizer_Who_Actioned_3");
                fields.Add("Authorizr4 as Authorizer_Who_Actioned_4");
                fields.Add("Authorizr5 as Authorizer_Who_Actioned_5");

                fields.Add("AuthStat1 as Notification_or_Authorization_Status_Code_1");
                fields.Add("AuthStat2 as Notification_or_Authorization_Status_Code_2");
                fields.Add("AuthStat3 as Notification_or_Authorization_Status_Code_3");
                fields.Add("AuthStat4 as Notification_or_Authorization_Status_Code_4");
                fields.Add("AuthStat5 as Notification_or_Authorization_Status_Code_5");

                fields.Add("ApvReason1 as Approval_Reason_1");
                fields.Add("ApvReason2 as Approval_Reason_2");
                fields.Add("ApvReason3 as Approval_Reason_3");
                fields.Add("ApvReason4 as Approval_Reason_4");
                fields.Add("ApvReason5 as Approval_Reason_5");

                fields.Add("TotTripChg as Total_Trip_Charge");
                fields.Add("AirChg as Air_Charge");
                fields.Add("AirLowFare as Lowest_Offered_Fare");
                fields.Add("AirLostSvg as Lost_Savings");

                fields.Add("AirExcReas as Reason_Code");
                fields.Add("Exchange");
                fields.Add("PenaltyAmt as Penalty_Amt");

                fields.Add("AirFareBkd as Booked_Fare");
                fields.Add("TktOrgAmt as Original_Ticket_Amount");

                fields.Add("AddCollAmt as Add_Collect");
                fields.Add("Class");
                fields.Add("ClsCatDesc as Class_Category_Description");
                fields.Add("Airline");
                fields.Add("RDepDate as Flight_Departure_Date");

                fields.Add("Origin as Origin_Airport_Code");
                fields.Add("OrgDesc as Origin_Airport_Name");
                fields.Add("Destinat as Destination_Airport_Code");
                fields.Add("DestDesc as Destination_Airport_Name");

                fields.Add("CarCompany as Car_Company_Name");
                fields.Add("RentDate as Car_Rental_Date");
                fields.Add("CarBookRat as Car_Booked_Rate");

                fields.Add("Days as Car_Rental_Days");
                fields.Add("CarCost as Total_Car_Cost");

                if (includeLostSavings)
                {
                    fields.Add("CarLowRate");
                    fields.Add("CarLostSvg");
                }

                fields.Add("HotelNam as Hotel_Name");
                fields.Add("Hotcity as Hotel_City");
                fields.Add("DateIn as Hotel_Check_in_Date");

                fields.Add("Rooms as Nbr_of_Hotel_Rooms");
                fields.Add("Nights as Nbr_of_Hotel_Nights");
                fields.Add("HotBookRat as Hotel_Booked_Rate");
                fields.Add("HotelCost as Total_Hotel_Cost");

                if (includeLostSavings)
                {
                    fields.Add("HotLowRate");
                    fields.Add("HotLostSvg");
                }

                return fields;
            }

            fields.Add("Reckey");
            fields.Add("TravAuthNo");
            fields.Add("RowNum");
            fields.Add("Acct");
            fields.Add("AcctName");
            fields.Add("break1 as " + globals.User.Break1Name);
            fields.Add("break2 as " + globals.User.Break2Name);
            fields.Add("break3 as " + globals.User.Break3Name);
            fields.Add("PassLast");

            fields.Add("PassFrst");
            fields.Add("RecLoc");
            fields.Add("bookedGMT as bookedDate");
            fields.Add("TravReason");
            fields.Add("CliAuthNbr");

            if (includeAuthComm) fields.Add("authcomm");

            fields.Add("StatusDate");
            fields.Add("StatusTime");
            fields.Add("StatusDesc");

            fields.Add("OOPReason1");
            fields.Add("OOPReason2");
            fields.Add("OOPReason3");
            fields.Add("OOPReason4");
            fields.Add("OOPReason5");

            fields.Add("AuthEmail1");
            fields.Add("AuthEmail2");
            fields.Add("AuthEmail3");
            fields.Add("AuthEmail4");
            fields.Add("AuthEmail5");

            fields.Add("Authorizr1");
            fields.Add("Authorizr2");
            fields.Add("Authorizr3");
            fields.Add("Authorizr4");
            fields.Add("Authorizr5");

            fields.Add("AuthStat1");
            fields.Add("AuthStat2");
            fields.Add("AuthStat3");
            fields.Add("AuthStat4");
            fields.Add("AuthStat5");

            fields.Add("ApvReason1");
            fields.Add("ApvReason2");
            fields.Add("ApvReason3");
            fields.Add("ApvReason4");
            fields.Add("ApvReason5");

            fields.Add("TotTripChg");
            fields.Add("AirChg");
            fields.Add("AirLowFare");
            fields.Add("AirLostSvg");
            fields.Add("AirExcReas");
            fields.Add("Exchange"); 

            fields.Add("PenaltyAmt");
            fields.Add("AirFareBkd");
            fields.Add("TktOrgAmt");
            fields.Add("AddCollAmt");
            fields.Add("Class");
            fields.Add("ClsCatDesc");
            fields.Add("Airline");
            fields.Add("RDepDate");
            fields.Add("Origin");


            fields.Add("OrgDesc");
            fields.Add("Destinat");
            fields.Add("DestDesc");
            fields.Add("CarCompany");
            fields.Add("RentDate");
            fields.Add("CarBookRat");
            fields.Add("Days");

            fields.Add("CarCost");

            if (includeLostSavings)
            {
                fields.Add("CarLowRate");
                fields.Add("CarLostSvg");
            }

            fields.Add("HotelNam");
            fields.Add("Hotcity");
            fields.Add("DateIn");
            fields.Add("Rooms");
            fields.Add("Nights");
            fields.Add("HotBookRat");
            fields.Add("HotelCost");

            if (includeLostSavings)
            {
                fields.Add("HotLowRate");
                fields.Add("HotLostSvg");
            }

            return fields;
        }
    }
}
