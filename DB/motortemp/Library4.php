
<?php

$StringDuty='Driver={SQL Server};Server=192.168.1.88;Database=Duty_Record';
$User='sa';
$Password='7312798';
function GetDeviceInformation()
{
	$order = array("\r\n", "\n", "\r","\\","\"");    
	$Json="{";
	 $sql='select * from tableinfo';
	 //$conn=odbc_connect($Connect_String,$User,$Password) or die("fault");
	 $conn=odbc_connect('Driver={SQL Server};Server=192.168.1.88;Database=Maintenance_Record',"sa","7312798") or die("fault");
	 $res=odbc_exec($conn,$sql) ;
	 $i=0;
	 while(odbc_fetch_row($res))
 {
	 $tablename[$i]=odbc_result($res,"tablename");
	 $tablenickname[$i]=odbc_result($res,"tablenickname"); 
	 $tablenickname[$i]=iconv('GB2312','UTF-8',$tablenickname[$i]);
	// echo  $tablenickname[$i];
	 $i++;
 }
     $countt=count($tablename);
	
	 for($i=0;$i<$countt;$i++)
	 {
		// echo $tablename[$i].":{";
		  $sql="select * from columninfo where belongtable='".$tablename[$i]."' order by itemorder";
		  $res=odbc_exec($conn,$sql) ;
		  $colname=array();
		  $colnickname=array();
		  $j=0;
	  while(odbc_fetch_row($res))		 
      {
       $colname[$j]=odbc_result($res,"colname");
	   $colnickname[$j]=odbc_result($res,"nickname");
	   $colnickname[$j]=iconv('GB2312','UTF-8',$colnickname[$j]);
	   $colnickname[$j]=str_replace($order,"",$colnickname[$j]); ;	
	   $j++;	
      }
	 // echo "},";
	 
	 
	 
	  $sql="select * from ".$tablename[$i];
      $res=odbc_exec($conn,$sql);
	  $count=count($colname);  
	 if($i==0) $Json.='"'.$tablenickname[$i].'":{';
	 else $Json.=',"'.$tablenickname[$i].'":{';
	 $k=0;
	 while(odbc_fetch_row($res))
	{
      $Line=odbc_result($res,"Line");
	  $Line=str_replace($order,"_",$Line);
	  $deviceid=odbc_result($res,"DeviceId");
	  $deviceid=str_replace($order,"_",$deviceid);
	  $Line=iconv('GB2312','UTF-8',$Line);
	  $deviceid=iconv('GB2312','UTF-8',$deviceid);
	 if($k==0) $Json.='"'.$deviceid."_".$Line.'":{';
	 else $Json.=',"'.$deviceid."_".$Line.'":{';
	 for($j=0;$j<$count;$j++)
	 {
		$content=iconv('GB2312','UTF-8',odbc_result($res,$colname[$j]));
		$content=str_replace(":"," ",$content);
		$content=str_replace("��"," ",$content);
	//	$content=str_replace(" ","_",$content);
		$content=str_replace($order,"_",$content);
		if($content=="") $content="�޴���Ϣ";
	    if($j==0) $Json.='"'.$colnickname[$j].'":"'.$content.'"';
		else $Json.=',"'.$colnickname[$j].'":"'.$content.'"';
		
	 }
	 $Json.='}';
	 $k++;	 
	 }
	 
	 $Json.="}";
	 }
	 $Json.="}";
	
	 odbc_close($conn);
	 echo $Json;
}
function closeconn($conn){
	echo iconv('GB2312','UTF-8',odbc_errormsg());

}
function GetDutyRecord($Line,$Interval,$Class,$Order,$Key,$Page)
{
	global $StringDuty,$User,$Password;
	//$String='Driver={SQL Server};Server=192.168.1.88;Database=Maintenance_Record';
    $conn=odbc_connect($StringDuty, $User,$Password) or die("fault");
	$Key="'%".$Key."%'";
	if($Line=="ȫ��")
	{
		$lineindex="and (TM3_Abnormal like ".$Key." or TM5_Abnormal like ".$Key." or TM10_Abnormal like ".$Key." or CS1_Abnormal like ".$Key." or CS2_Abnormal like ".$Key.")";
	}
	else
	{
		if($Line=="TM3")  $lineindex="and TM3_Abnormal like ".$Key;
		else if($Line=="TM5") $lineindex="and TM5_Abnormal like ".$Key;
		else if($Line=="TM10") $lineindex="and TM10_Abnormal like ".$Key;
		else if($Line=="����ֽ") $lineindex="and (CS1_Abnormal like ".$Key." or CS2_Abnormal like ".$Key.")";
		
	}
	if($Interval=="ȫ��")
	{
		$Interval="%%";
	}
	if($Class=="ȫ��")
	{
		$Class="%%";
	}
	if($Order=="ȫ��")
	{
		$Order="%%";
	}
	$Dateindex="and datediff(d,Date,getdate())<".$Interval;
    $index="  Class like '".$Class."' and ClassOrder like '".$Order."' ".$Dateindex.$lineindex;
    $sql="select top 15 * from DutyRecorAbnormal where item not in(select top (15*".$Page.") item from DutyRecorAbnormal where ".$index." order by item desc) and".$index." order by item desc" ;
	$res=odbc_exec($conn,$sql) or die( closeconn());
	$json="[";
	$count=0;
	$order = array("\r\n", "\n", "\r","\\","\""); 
	while(odbc_fetch_row($res)) 
	{
		$sqldate=iconv('GB2312','UTF-8',odbc_result($res,"Date"));
		$sqldate=date('Y-m-d',strtotime($sqldate));
		$sqldate=str_replace($order,"",$sqldate);
		
		$sqlclass=iconv('GB2312','UTF-8',odbc_result($res,"Class"));
		$sqlclass=str_replace($order,"",$sqlclass);
		
		$sqlorder=iconv('GB2312','UTF-8',odbc_result($res,"ClassOrder"));
		$sqlorder=str_replace($order,"",$sqlorder);
		
		$sqlname=iconv('GB2312','UTF-8',odbc_result($res,"Name"));
		$sqlname=str_replace($order,"",$sqlname);
		
		$sqltm3ab=iconv('GB2312','UTF-8',odbc_result($res,"TM3_Abnormal"));
		$sqltm3ab=str_replace($order,"",$sqltm3ab);
		
		$sqltm5ab=iconv('GB2312','UTF-8',odbc_result($res,"TM5_Abnormal"));
		$sqltm5ab=str_replace($order,"",$sqltm5ab);
		
		$sqltm10ab=iconv('GB2312','UTF-8',odbc_result($res,"TM10_Abnormal"));
		$sqltm10ab=str_replace($order,"",$sqltm10ab);
		
		$sqlcs1ab=iconv('GB2312','UTF-8',odbc_result($res,"CS1_Abnormal"));
		$sqlcs1ab=str_replace($order,"",$sqlcs1ab);
		
		$sqlcs2ab=iconv('GB2312','UTF-8',odbc_result($res,"CS2_Abnormal"));
		$sqlcs2ab=str_replace($order,"",$sqlcs2ab);
		
		$sqlanotherab=iconv('GB2312','UTF-8',odbc_result($res,"AntherRecord"));
		$sqlanotherab=str_replace($order,"",$sqlanotherab);
		
		$sqltm3f=iconv('GB2312','UTF-8',odbc_result($res,"TM3_Force"));
		$sqltm3f=str_replace($order,"",$sqltm3f);
		
		$sqltm5f=iconv('GB2312','UTF-8',odbc_result($res,"TM5_Force"));
		$sqltm5f=str_replace($order,"",$sqltm5f);
		
		$sqltm10f=iconv('GB2312','UTF-8',odbc_result($res,"TM10_Force"));
		$sqltm10f=str_replace($order,"",$sqltm10f);
		
		$sqlcsf=iconv('GB2312','UTF-8',odbc_result($res,"CS_Force"));
		$sqlcsf=str_replace($order,"",$sqlcsf);
		
		$subjson='{'.'"date":"'.$sqldate.'",'.'"class":"'.$sqlclass.'",'.'"order":"'.$sqlorder.'",'.'"name":"'.$sqlname.'",'.'"tm3ab":"'.$sqltm3ab.'",'
		          .'"tm5ab":"'.$sqltm5ab.'",'.'"tm10ab":"'.$sqltm10ab.'",'.'"cs1ab":"'.$sqlcs1ab.'",'.'"cs2ab":"'.$sqlcs2ab.'",'.'"anotherab":"'.$sqlanotherab.'",'
				  .'"tm3f":"'.$sqltm3f.'",'.'"tm5f":"'.$sqltm5f.'",'.'"tm10f":"'.$sqltm10f.'",'.'"csf":"'.$sqlcsf.'"'."}";
	    if($count==0)
		$json.=$subjson;
		else
		$json.=",".$subjson;
		$count++;
	}
	$json.="]";
	//$json=str_replace($order,"",$json);
	echo $json;
	odbc_close($conn);

}
?>