<?php include  'CabinetRecord_Library.php';
date_default_timezone_set('Asia/Shanghai');  ?>
<?php
/**********************功能区******************************/
function geterror()
{
    $msg=odbc_errormsg();
    $file=fopen("msg.txt","a+");
	fwrite($file,odbc_errormsg());
	fclose($file);
}
  class CabinetRecord extends SqlConnection
{
	private $userid,$password;
	private $xml;
	private $data,$name;
	function CabinetRecord($data,$name)
	{
       $this->xml=new ParseXML($data,$name);
	   $this->SqlConnection(CONNECTIONSTRING,USER,PASSWORD);  
	}
	 protected function GetData($con){
     while($this->xml->GetNextCabinet())
     {
	   $sql=$this->xml->GetSQL(); 
	   odbc_exec($con,$sql) ;
     }
    echo "FILE SEND SUCCESS";
	}
}
?>
<?php
/**********************运行区*****************************/
$name=$_POST["name"];
$data=$_POST["data"];
 
//$name=iconv('utf-8','GB2312',$name);
//$file=fopen("msg.txt","w");
//fwrite($file,$name);
//fclose($file);
$cab=new CabinetRecord($data,$name);

?>