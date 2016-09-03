<?php include  'CabinetRecord_Library.php';
date_default_timezone_set('Asia/Shanghai');  
?>

<?php
/********************功能区************************/
class NetConnect extends SqlConnection
{
	private $id;
	function NetConnect($cabinetid)
	{
	   $this->id=$cabinetid;
	  $this->SqlConnection(CONNECTIONSTRING,USER,PASSWORD);
	}
	protected function GetData($con){
		$res=odbc_exec($con,"select * from sbzlb where DeviceId='".$this->id."' ");
	
	   if(odbc_fetch_row($res))		 
      {
		  echo iconv('GB2312','UTF-8',odbc_result($res,"id"));
	  }
	  else
	  {
		  echo "nodevice";
	  }
	}
}
?>
<?php
/*********************运行区****************************/
   $cabinetid=$_GET["cabinetid"];
   $clas=new NetConnect($cabinetid);
?>


