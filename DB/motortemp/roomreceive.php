<?php
function closeconn($conn,$time,$class,$key,$temp,$mois,$remark){
	$sql="update DutyRecordTemp set Temperature='$temp',Moisture='$mois',Remark='$remark' where Date='$time' and Class='$class' and Position='$key'";
        odbc_exec($conn,$sql) or die('fault');                                                                                      
	//odbc_close($conn);
      //  echo "FILE SEND SUCCESS";
}

?>
<?php
        $confirm=$_POST['confirm'];
       if($confirm!="RoomTemp")
	{
	   die('fault');
	}
        date_default_timezone_set('PRC');//
         $conn=odbc_connect('Driver={SQL Server};Server=172.16.132.192\PARTS_MANAGE;Database=Duty_Record',"sa","Luoqing123") or die('fault');
         $class=$_POST['class'];
		$json=$_POST["json"];
		$class=iconv('UTF-8','GB2312',$class);
		$arr=json_decode($json,TRUE);
		$arr_key=array_keys($arr);
		$count=count($arr_key);
		for($i=0;$i<$count;$i++){
			$key=$arr_key[$i];
			$temp=$arr[$key]['temp'];
			$mois=$arr[$key]['mois'];
			$time=date('Y-m-d H:0:0',time());
			$remark=iconv('UTF-8','GB2312',$arr[$key]['remark']);
			 $sql="insert into DutyRecordTemp values('$time','$class','$key','$temp','$mois','$remark')";
			 odbc_exec($conn,$sql) or closeconn($conn,$time,$class,$key,$temp,$mois,$remark);
		}
		
        odbc_close($conn);
       echo "FILE SEND SUCCESS";
		
          
?>