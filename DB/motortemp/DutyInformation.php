<?php include_once 'Library.php' ?>
<?php


$Line=$_GET['line'];
$Class=$_GET['class'];
$Interval=$_GET['interval'];
$Order=$_GET['order'];
$Key=$_GET['key'];
$Page=$_GET['page'];

$Line=iconv('UTF-8','GB2312',$Line);
$Class=iconv('UTF-8','GB2312',$Class);
$Interval=iconv('UTF-8','GB2312',$Interval);
$Order=iconv('UTF-8','GB2312',$Order);
$Key=iconv('UTF-8','GB2312',$Key);

GetDutyRecord($Line,$Interval,$Class,$Order,$Key,$Page);
//echo $Line.$Class.$Interval.$Order.$Key.$Page;
?>
