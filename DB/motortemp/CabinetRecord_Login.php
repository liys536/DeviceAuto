<?php include  'CabinetRecord_Library.php';
 ?>
<?php
/********************功能区************************/
class CabinetRecordLogin extends SqlConnection
{
	private $userid,$password;
	function CabinetRecordLogin($Id,$Password)
	{
	   $this->userid=$Id;
	   $this->password=$Password;
	  $this->SqlConnection(CONNECTIONSTRING,USER,PASSWORD);
	}
	protected function GetData($con){
		$res=odbc_exec($con,"select * from ygzlb where cYgbh='".$this->userid."' and cDlmm='".$this->password."'");
	
	   if(odbc_fetch_row($res))		 
      {
		  echo iconv('GB2312','UTF-8',odbc_result($res,"cYgxm" ));
	  }
	  else
	  {
		  echo "nobody";
	  }
	}
}
?>
<?php
/*********************运行区****************************/
   $id=$_GET["id"];
   $passowrd=$_GET["pas"];
   $clas=new CabinetRecordLogin($id,$passowrd);
?>