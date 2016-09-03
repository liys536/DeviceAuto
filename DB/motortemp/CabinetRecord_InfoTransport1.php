<?php include  'CabinetRecord_Library.php'; ?>
<?php
/**********************功能区***************************************/

class CabinetRecordToXML extends SqlConnection
{
	private $Dom;
	private $CabinetRecord;
	public function CabinetRecordToXML()
	{
		$this->Dom=new DOMDocument('1.0','utf-8');
		$this->SqlConnection(CONNECTIONSTRING,USER,PASSWORD);
	}
	protected function GetData($con)
	{
	    $this->GetCabinetType($con);
		$this->GetCabinetTypemember($con);
	}
	private function GetCabinetType($con)
	{
		$this->CabinetRecord=$this->Dom->createElement('CabinetRecord');
	   $RecordTypes=$this->Dom->createElement('RecordTypes');
	   $this->CabinetRecord->appendChild($RecordTypes);
	   $this->Dom->appendChild($this->CabinetRecord);
		$res=odbc_exec($con,"select * from ".CabinetRecordType);
		 while(odbc_fetch_row($res))		 
      {
		$name=iconv('GB2312','UTF-8',odbc_result($res,"TypeName"));
		$CabinetType=$this->Dom->createElement("TYPE") ;
		$CabinetType->setAttribute("TypeName",$name);
		$designer=$this->Dom->createAttribute("Designer");
		$designer->appendChild($this->Dom->createtextNode(iconv('GB2312','UTF-8',odbc_result($res,"TypeDesigner"    ))));
		$CabinetType->appendChild($designer);
	/*	$CabinetType->appendChild($this->Dom->createtextNode(iconv('GB2312','UTF-8',odbc_result($res,      "ColumnInformation"))));*/
	    $Colinfo=$this->Dom->createElement("Colinfo");
	    $dom=new DOMDocument();
		$dom->loadXML(iconv('GB2312','UTF-8',odbc_result($res,"ColumnInformation")));
		foreach($dom->childNodes as $node)
		  foreach($node->childNodes as $childnode)
		  {
			  $colname=$this->Dom->createElement($childnode->nodeName);
			  $colname->nodeValue=$childnode->nodeValue;
			  $Colinfo->appendChild($colname);
		  }
		$CabinetType->appendChild($Colinfo);
		$RecordTypes->appendChild($CabinetType);
	  }
		return trim($this->Dom->saveXML());
	  } 
	
	private function GetCabinetTypemember($con)
	{
		$res=odbc_exec($con,"select a.*,b.TypeName from sbzlb a left join CabinetRecordType b on a.iDjid = b.id  where a.iDjid is not null ");
		$typemember=$this->Dom->createElement("Typemember");
		$this->CabinetRecord->appendChild($typemember);
		 while(odbc_fetch_row($res))		 
         {	
		     // odbc_result($res,"CabinetID");
			  $id=iconv('GB2312','UTF-8',odbc_result($res,"DeviceId"));
			  $cabinet=$this->Dom->createElement("CABINET");
			  $cabinet->setAttribute("CabinetId",$id);
			  $cabinet->setAttribute("CabinetName",iconv('GB2312','UTF-8',odbc_result($res,"DeviceName")));
			  $cabinet->setAttribute("Line",iconv('GB2312','UTF-8',odbc_result($res,"Line")));
			  $cabinet->setAttribute("belong",iconv('GB2312','UTF-8',odbc_result($res,"belong")));	
			  $cabinet->setAttribute("TypeName",iconv('GB2312','UTF-8',odbc_result($res,"TypeName")));			
			  $typemember->appendChild($cabinet);
		 }
		  echo  trim($this->Dom->saveXML());
	}
}
?>
<?php
/***********************运行区********************************/
$xml=new CabinetRecordToXML();
?>
