<?php
function jsonEncodeWithCN($data) {
return preg_replace("/\\\u([0-9a-f]{4})/ie", "iconv('UCS-2BE', 'UTF-8', pack('H4', '$1'))", $data);
}
?>
<?php
include 'excel_lib.php';
$json=$_POST["json"];
$line=$_POST["line"];
$filename=iconv('UTF-8','GB2312',$line).".json";
if (file_exists($filename)==false) 
{
//$json=iconv('UTF-8','GB2312',$json);
$myfile = fopen($filename, "w");
fwrite($myfile, $json);
fclose($myfile);
$result="FILE SEND SUCCESS"; 
echo $result;
}
else
{
	
	$myfile = fopen($filename, "r+");
	$json0=fread($myfile,filesize($filename));
	fclose($myfile);
//	$json0=iconv('GB2312','UTF-8',$json0);
	$json1=json_decode($json0,true);//老数据
	$json2=json_decode($json,true);//新传入
	$key=array_keys($json2);
        foreach($json2 as $j){
		foreach($key as $k)
		{
		
				$json1[$k]=$json2[$k];
		
		}
	}
	$myfile = fopen($filename, "w");
	$json1=json_encode($json1);
        $json1=jsonEncodeWithCN($json1);
	fwrite($myfile, $json1);
	fclose($myfile);	
	echo "FILE SEND SUCCESS";

}


?>
