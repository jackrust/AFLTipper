<?xml version="1.0" encoding="utf-8"?>
<wsdl:definitions xmlns:s="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://schemas.xmlsoap.org/wsdl/soap12/" xmlns:mime="http://schemas.xmlsoap.org/wsdl/mime/" xmlns:tns="http://xml.afl.com.au" xmlns:s1="http://microsoft.com/wsdl/types/" xmlns:soap="http://schemas.xmlsoap.org/wsdl/soap/" xmlns:tm="http://microsoft.com/wsdl/mime/textMatching/" xmlns:http="http://schemas.xmlsoap.org/wsdl/http/" xmlns:soapenc="http://schemas.xmlsoap.org/soap/encoding/" targetNamespace="http://xml.afl.com.au" xmlns:wsdl="http://schemas.xmlsoap.org/wsdl/">
  <wsdl:types>
    <s:schema elementFormDefault="qualified" targetNamespace="http://xml.afl.com.au">
      <s:import namespace="http://microsoft.com/wsdl/types/" />
      <s:element name="GetVersion">
        <s:complexType />
      </s:element>
      <s:element name="GetVersionResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="GetVersionResult" type="tns:ServiceVersion" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="ServiceVersion">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="Version" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="AnnouncementsByCompetitionRequest" type="tns:AnnouncementsByCompetitionRequest" />
      <s:complexType name="AnnouncementsByCompetitionRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="CompetitionId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="AnnouncementsByCompetitionResponse" type="tns:AnnouncementsByCompetitionResponse" />
      <s:complexType name="AnnouncementsByCompetitionResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="AnnouncementList" type="tns:ArrayOfAnnouncement" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="BaseResponse">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="TTL" type="s:dateTime" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="ArrayOfAnnouncement">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="Announcement" nillable="true" type="tns:Announcement" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="Announcement">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Title" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="Description" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="Link" type="tns:Url" />
          <s:element minOccurs="1" maxOccurs="1" name="DisplayOrder" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="ImageUrl" type="tns:Url" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="Url">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="Name" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="Link" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="HighResLink" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="AnnouncementsByTeamRequest" type="tns:AnnouncementsByTeamRequest" />
      <s:complexType name="AnnouncementsByTeamRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="TeamId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="AnnouncementsByTeamResponse" type="tns:AnnouncementsByTeamResponse" />
      <s:complexType name="AnnouncementsByTeamResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="AnnouncementList" type="tns:ArrayOfAnnouncement" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="GetMediaProgramTypes">
        <s:complexType />
      </s:element>
      <s:element name="GetMediaProgramTypesResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="MediaProgramTypeResponse" type="tns:MediaProgramTypeResponse" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="MediaProgramTypeResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="ProgramTypeList" type="tns:ArrayOfMediaProgramType" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="ArrayOfMediaProgramType">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="MediaProgramType" nillable="true" type="tns:MediaProgramType" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="MediaProgramType">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Title" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="VideosByCompetitionCRequest" type="tns:VideosByCompetitionCRequest" />
      <s:complexType name="VideosByCompetitionCRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="CompetitionId" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfVideos" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramTypeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="VideosByCompetitionCResponse" type="tns:VideosByCompetitionCResponse" />
      <s:complexType name="VideosByCompetitionCResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="VideoList" type="tns:ArrayOfVideoC" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="ArrayOfVideoC">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="VideoC" nillable="true" type="tns:VideoC" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="VideoC">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Title" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="Description" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="PublishedDate" type="s:dateTime" />
          <s:element minOccurs="0" maxOccurs="1" name="Provider" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="ImageUrl" type="tns:Url" />
          <s:element minOccurs="0" maxOccurs="1" name="VideoUrl" type="tns:Url" />
          <s:element minOccurs="1" maxOccurs="1" name="RelatedOnlineVideo" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramType" type="s:int" />
        </s:sequence>
      </s:complexType>
      <s:element name="VideosByCompetitionRequest" type="tns:VideosByCompetitionRequest" />
      <s:complexType name="VideosByCompetitionRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="CompetitionId" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfVideos" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramTypeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="VideosByCompetitionResponse" type="tns:VideosByCompetitionResponse" />
      <s:complexType name="VideosByCompetitionResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="VideoList" type="tns:ArrayOfVideo" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="ArrayOfVideo">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="Video" nillable="true" type="tns:Video" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="Video">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Title" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="Description" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="PublishedDate" type="s:dateTime" />
          <s:element minOccurs="0" maxOccurs="1" name="Provider" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="ImageUrl" type="tns:Url" />
          <s:element minOccurs="0" maxOccurs="1" name="VideoUrl" type="tns:Url" />
        </s:sequence>
      </s:complexType>
      <s:element name="VideosByTeamCRequest" type="tns:VideosByTeamCRequest" />
      <s:complexType name="VideosByTeamCRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="TeamId" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfVideos" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramTypeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="VideosByTeamCResponse" type="tns:VideosByTeamCResponse" />
      <s:complexType name="VideosByTeamCResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="VideoList" type="tns:ArrayOfVideoC" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="VideosByTeamRequest" type="tns:VideosByTeamRequest" />
      <s:complexType name="VideosByTeamRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="TeamId" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfVideos" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramTypeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="VideosByTeamResponse" type="tns:VideosByTeamResponse" />
      <s:complexType name="VideosByTeamResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="VideoList" type="tns:ArrayOfVideo" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="VideosByFixtureCRequest" type="tns:VideosByFixtureCRequest" />
      <s:complexType name="VideosByFixtureCRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="FixtureId" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfVideos" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramTypeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="VideosByFixtureCResponse" type="tns:VideosByFixtureCResponse" />
      <s:complexType name="VideosByFixtureCResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="VideoList" type="tns:ArrayOfVideoC" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="VideosByFixtureRequest" type="tns:VideosByFixtureRequest" />
      <s:complexType name="VideosByFixtureRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="FixtureId" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfVideos" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramTypeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="VideosByFixtureResponse" type="tns:VideosByFixtureResponse" />
      <s:complexType name="VideosByFixtureResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="VideoList" type="tns:ArrayOfVideo" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="VideosByIdRequest" type="tns:VideosByIdRequest" />
      <s:complexType name="VideosByIdRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="VideoId" type="s:int" />
        </s:sequence>
      </s:complexType>
      <s:element name="VideosByIdResponse" type="tns:VideosByIdResponse" />
      <s:complexType name="VideosByIdResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="VideoList" type="tns:ArrayOfVideo" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="VideosByIdCRequest" type="tns:VideosByIdCRequest" />
      <s:complexType name="VideosByIdCRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="VideoId" type="s:int" />
        </s:sequence>
      </s:complexType>
      <s:element name="VideosByIdCResponse" type="tns:VideosByIdCResponse" />
      <s:complexType name="VideosByIdCResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="VideoList" type="tns:ArrayOfVideoC" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="OnlineVideosByCompetitionCRequest" type="tns:OnlineVideosByCompetitionCRequest" />
      <s:complexType name="OnlineVideosByCompetitionCRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="CompetitionId" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfVideos" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramTypeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="OnlineVideosByCompetitionResponse" type="tns:OnlineVideosByCompetitionResponse" />
      <s:complexType name="OnlineVideosByCompetitionResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="VideoList" type="tns:ArrayOfOnlineVideo" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="ArrayOfOnlineVideo">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="OnlineVideo" nillable="true" type="tns:OnlineVideo" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="OnlineVideo">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="StreamingUrlList" type="tns:ArrayOfMediaUrl" />
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Title" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="Description" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="PublishedDate" type="s:dateTime" />
          <s:element minOccurs="0" maxOccurs="1" name="Provider" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="ImageUrl" type="tns:Url" />
          <s:element minOccurs="1" maxOccurs="1" name="RelatedVideo" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramType" type="s:int" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="ArrayOfMediaUrl">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="MediaUrl" nillable="true" type="tns:MediaUrl" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="MediaUrl">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="Name" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="Link" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="BitRate" type="tns:BitRate" />
        </s:sequence>
      </s:complexType>
      <s:simpleType name="BitRate">
        <s:restriction base="s:string">
          <s:enumeration value="Low" />
          <s:enumeration value="Med" />
          <s:enumeration value="High" />
        </s:restriction>
      </s:simpleType>
      <s:element name="OnlineVideosByCompetitionRequest" type="tns:OnlineVideosByCompetitionRequest" />
      <s:complexType name="OnlineVideosByCompetitionRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="CompetitionId" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfVideos" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramTypeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="OnlineVideosByTeamCRequest" type="tns:OnlineVideosByTeamCRequest" />
      <s:complexType name="OnlineVideosByTeamCRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="TeamId" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfVideos" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramTypeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="OnlineVideosByTeamResponse" type="tns:OnlineVideosByTeamResponse" />
      <s:complexType name="OnlineVideosByTeamResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="VideoList" type="tns:ArrayOfOnlineVideo" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="OnlineVideosByTeamRequest" type="tns:OnlineVideosByTeamRequest" />
      <s:complexType name="OnlineVideosByTeamRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="TeamId" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfVideos" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramTypeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="OnlineVideosByFixtureCRequest" type="tns:OnlineVideosByFixtureCRequest" />
      <s:complexType name="OnlineVideosByFixtureCRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="FixtureId" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfVideos" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramTypeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="OnlineVideosByFixtureResponse" type="tns:OnlineVideosByFixtureResponse" />
      <s:complexType name="OnlineVideosByFixtureResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="VideoList" type="tns:ArrayOfOnlineVideo" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="OnlineVideosByFixtureRequest" type="tns:OnlineVideosByFixtureRequest" />
      <s:complexType name="OnlineVideosByFixtureRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="FixtureId" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfVideos" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramTypeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="GetOnlineVideosById">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="OnlineVideosByIdRequest" type="tns:OnlineVideosByIdRequest" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="OnlineVideosByIdRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="VideoId" type="s:int" />
        </s:sequence>
      </s:complexType>
      <s:element name="GetOnlineVideosByIdResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="OnlineVideosByIdResponse" type="tns:OnlineVideosByIdResponse" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="OnlineVideosByIdResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="VideoList" type="tns:ArrayOfOnlineVideo" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="RelatedVideosByVideoRequest" type="tns:RelatedVideosByVideoRequest" />
      <s:complexType name="RelatedVideosByVideoRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="VideoId" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfVideos" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="ProgramTypeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="RelatedVideosByVideoResponse" type="tns:RelatedVideosByVideoResponse" />
      <s:complexType name="RelatedVideosByVideoResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="VideoList" type="tns:ArrayOfVideo" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="GetNewsCategories">
        <s:complexType />
      </s:element>
      <s:element name="GetNewsCategoriesResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="NewsCategoriesResponse" type="tns:NewsCategoriesResponse" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="NewsCategoriesResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="CategoryList" type="tns:ArrayOfNewsCategory" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="ArrayOfNewsCategory">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="NewsCategory" nillable="true" type="tns:NewsCategory" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="NewsCategory">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Title" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="NewsArticleByCompetitionRequest" type="tns:NewsArticlesByCompetitionRequest" />
      <s:complexType name="NewsArticlesByCompetitionRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="CompetitionId" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfArticles" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="WapArticles" type="s:boolean" />
          <s:element minOccurs="0" maxOccurs="1" name="Options" type="tns:MediaOption" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="MediaOption">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="IsMediaRelease" type="s:boolean" />
          <s:element minOccurs="1" maxOccurs="1" name="IsTopNews" type="s:boolean" />
          <s:element minOccurs="1" maxOccurs="1" name="IsFeatured" type="s:boolean" />
          <s:element minOccurs="1" maxOccurs="1" name="IsCasualty" type="s:boolean" />
          <s:element minOccurs="1" maxOccurs="1" name="IsTribunal" type="s:boolean" />
          <s:element minOccurs="1" maxOccurs="1" name="IsCoaching" type="s:boolean" />
          <s:element minOccurs="1" maxOccurs="1" name="IsTrade" type="s:boolean" />
          <s:element minOccurs="1" maxOccurs="1" name="IsDraft" type="s:boolean" />
          <s:element minOccurs="1" maxOccurs="1" name="IsRecord" type="s:boolean" />
          <s:element minOccurs="1" maxOccurs="1" name="IsBlog" type="s:boolean" />
          <s:element minOccurs="1" maxOccurs="1" name="IsMobileApp" type="s:boolean" />
        </s:sequence>
      </s:complexType>
      <s:element name="NewsArticleByCompetitionResponse" type="tns:NewsArticlesByCompetitionResponse" />
      <s:complexType name="NewsArticlesByCompetitionResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="NewsArticleList" type="tns:ArrayOfNewsArticle" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="ArrayOfNewsArticle">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="NewsArticle" nillable="true" type="tns:NewsArticle" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="NewsArticle">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="ByLine" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="isFeature" type="s:boolean" />
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Heading" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="Abstract" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="Description" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="PublishedDate" type="s:dateTime" />
          <s:element minOccurs="0" maxOccurs="1" name="Source" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="ArticleUrl" type="tns:Url" />
          <s:element minOccurs="0" maxOccurs="1" name="ImageUrls" type="tns:ArrayOfUrl" />
          <s:element minOccurs="0" maxOccurs="1" name="VideoUrls" type="tns:ArrayOfUrl" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="ArrayOfUrl">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="Url" nillable="true" type="tns:Url" />
        </s:sequence>
      </s:complexType>
      <s:element name="NewsArticleByTeamRequest" type="tns:NewsArticlesByTeamRequest" />
      <s:complexType name="NewsArticlesByTeamRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="TeamId" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfArticles" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="WapArticles" type="s:boolean" />
          <s:element minOccurs="0" maxOccurs="1" name="Options" type="tns:MediaOption" />
        </s:sequence>
      </s:complexType>
      <s:element name="NewsArticleByTeamResponse" type="tns:NewsArticlesByTeamResponse" />
      <s:complexType name="NewsArticlesByTeamResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="NewsArticleList" type="tns:ArrayOfNewsArticle" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="NewsArticlesByFixtureRequest" type="tns:NewsArticlesByFixtureRequest" />
      <s:complexType name="NewsArticlesByFixtureRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="FixtureId" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfArticles" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="WapArticles" type="s:boolean" />
          <s:element minOccurs="0" maxOccurs="1" name="Options" type="tns:MediaOption" />
        </s:sequence>
      </s:complexType>
      <s:element name="NewsArticlesByFixtureResponse" type="tns:NewsArticlesByFixtureResponse" />
      <s:complexType name="NewsArticlesByFixtureResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="NewsArticleList" type="tns:ArrayOfNewsArticle" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="NewsArticleByIdRequest" type="tns:NewsArticleByIdRequest" />
      <s:complexType name="NewsArticleByIdRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="NewsId" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="WapArticles" type="s:boolean" />
        </s:sequence>
      </s:complexType>
      <s:element name="NewsArticleByIdResponse" type="tns:NewsArticleByIdResponse" />
      <s:complexType name="NewsArticleByIdResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="NewsArticle" type="tns:NewsArticle" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="RelatedNewsArticlesByNewsRequest" type="tns:RelatedNewsArticlesByNewsRequest" />
      <s:complexType name="RelatedNewsArticlesByNewsRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="NewsId" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfArticles" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="WapArticles" type="s:boolean" />
          <s:element minOccurs="0" maxOccurs="1" name="Options" type="tns:MediaOption" />
        </s:sequence>
      </s:complexType>
      <s:element name="RelatedNewsArticlesByNewsResponse" type="tns:RelatedNewsArticlesByNewsResponse" />
      <s:complexType name="RelatedNewsArticlesByNewsResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="NewsArticleList" type="tns:ArrayOfNewsArticle" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="CrossProductLinksByCompetitionRequest" type="tns:CrossProductLinksByCompetitionRequest" />
      <s:complexType name="CrossProductLinksByCompetitionRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="CompetitionId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="CrossProductLinksByCompetitionResponse" type="tns:CrossProductLinksByCompetitionResponse" />
      <s:complexType name="CrossProductLinksByCompetitionResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="UrlList" type="tns:ArrayOfUrl" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="LadderRequest" type="tns:LadderBySeasonRequest" />
      <s:complexType name="LadderBySeasonRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="SeasonId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="LadderResponse" type="tns:LadderBySeasonResponse" />
      <s:complexType name="LadderBySeasonResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="LadderList" type="tns:ArrayOfLadderItem" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="ArrayOfLadderItem">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="LadderItem" nillable="true" type="tns:LadderItem" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="LadderItem">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Rank" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Team" type="tns:Team" />
          <s:element minOccurs="1" maxOccurs="1" name="Played" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="Won" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="Lost" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="Draw" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="Points" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="SeasonFor" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="SeasonAgainst" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="Difference" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="Percentage" type="s:decimal" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="Team">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:short" />
          <s:element minOccurs="0" maxOccurs="1" name="Name" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="Nickname" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="Abbreviation" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfJerseys" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="ImageUrl" type="tns:Url" />
        </s:sequence>
      </s:complexType>
      <s:element name="TeamsRequest" type="tns:TeamsBySeasonRequest" />
      <s:complexType name="TeamsBySeasonRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="SeasonId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="TeamsResponse" type="tns:TeamsBySeasonResponse" />
      <s:complexType name="TeamsBySeasonResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="TeamList" type="tns:ArrayOfTeam" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="ArrayOfTeam">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="Team" nillable="true" type="tns:Team" />
        </s:sequence>
      </s:complexType>
      <s:element name="PlayersByTeamRequest" type="tns:PlayersByTeamRequest" />
      <s:complexType name="PlayersByTeamRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="TeamId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="PlayersByTeamResponse" type="tns:PlayersByTeamResponse" />
      <s:complexType name="PlayersByTeamResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="PlayerList" type="tns:ArrayOfPlayer" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="ArrayOfPlayer">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="Player" nillable="true" type="tns:Player" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="Player">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="FamilyName" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="GivenNames" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="NickName" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="ImageUrl" type="tns:Url" />
          <s:element minOccurs="0" maxOccurs="1" name="CasualtyList" type="tns:ArrayOfCasualty" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="ArrayOfCasualty">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="Casualty" nillable="true" type="tns:Casualty" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="Casualty">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="RoundId" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Injury" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="InjuryDate" type="s:dateTime" />
          <s:element minOccurs="1" maxOccurs="1" name="ReturnDate" type="s:dateTime" />
          <s:element minOccurs="0" maxOccurs="1" name="EstimatedWeeksOut" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="InjuryDetail" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="PlayersByTeamAndFixtureRequest" type="tns:PlayersByTeamAndFixtureRequest" />
      <s:complexType name="PlayersByTeamAndFixtureRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="FixtureId" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="TeamId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="PlayersByTeamAndFixtureResponse" type="tns:PlayersByTeamAndFixtureResponse" />
      <s:complexType name="PlayersByTeamAndFixtureResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="PlayerList" type="tns:ArrayOfPlayer" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="FixturesByRoundRequest" type="tns:FixturesByRoundRequest" />
      <s:complexType name="FixturesByRoundRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="RoundId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="FixturesByRoundResponse" type="tns:FixturesByRoundResponse" />
      <s:complexType name="FixturesByRoundResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="FixtureList" type="tns:ArrayOfFixture" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="ArrayOfFixture">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="Fixture" nillable="true" type="tns:Fixture" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="Fixture">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="StatsId" type="s:long" />
          <s:element minOccurs="0" maxOccurs="1" name="Status" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="IsBye" type="s:boolean" />
          <s:element minOccurs="0" maxOccurs="1" name="Venue" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="VenueOffset" type="s:float" />
          <s:element minOccurs="0" maxOccurs="1" name="HomeTeam" type="tns:Team" />
          <s:element minOccurs="0" maxOccurs="1" name="AwayTeam" type="tns:Team" />
          <s:element minOccurs="1" maxOccurs="1" name="LocalStartTime" type="s:dateTime" />
          <s:element minOccurs="1" maxOccurs="1" name="HomeGoals" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="HomeSuperGoals" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="HomeBehinds" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="HomeScore" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="AwayGoals" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="AwaySuperGoals" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="AwayBehinds" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="AwayScore" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="LastUpdated" type="s:dateTime" />
          <s:element minOccurs="1" maxOccurs="1" name="PeriodSeconds" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="ImageUrl" type="tns:Url" />
          <s:element minOccurs="1" maxOccurs="1" name="InProgress" type="s:boolean" />
          <s:element minOccurs="0" maxOccurs="1" name="Title" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="FixturesbySeasonRequest" type="tns:FixturesBySeasonRequest" />
      <s:complexType name="FixturesBySeasonRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="SeasonId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="FixturesbySeasonResponse" type="tns:FixturesBySeasonResponse" />
      <s:complexType name="FixturesBySeasonResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="FixtureList" type="tns:ArrayOfFixture" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="FixturesBySeasonAndTeamRequest" type="tns:FixturesBySeasonAndTeamRequest" />
      <s:complexType name="FixturesBySeasonAndTeamRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="SeasonId" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="TeamId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="FixturesBySeasonAndTeamResponse" type="tns:FixturesBySeasonAndTeamResponse" />
      <s:complexType name="FixturesBySeasonAndTeamResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="FixtureList" type="tns:ArrayOfFixture" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="FixtureByIdRequest" type="tns:FixtureByIdRequest" />
      <s:complexType name="FixtureByIdRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="FixtureId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="FixtureByIdResponse" type="tns:FixtureByIdResponse" />
      <s:complexType name="FixtureByIdResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="Fixture" type="tns:Fixture" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="RankingByTeamRequest" type="tns:RankingByTeamRequest" />
      <s:complexType name="RankingByTeamRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="TeamId" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="SeasonId" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfRecordsPerCategory" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="RankingByTeamResponse" type="tns:RankingByTeamResponse" />
      <s:complexType name="RankingByTeamResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="RankingList" type="tns:ArrayOfRank" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="ArrayOfRank">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="Rank" nillable="true" type="tns:Rank" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="Rank">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="DataTypeId" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="DataTypeName" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="Team" type="tns:Team" />
          <s:element minOccurs="0" maxOccurs="1" name="Player" type="tns:Player" />
          <s:element minOccurs="1" maxOccurs="1" name="ItemValue" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="LastUpdated" type="s:dateTime" />
        </s:sequence>
      </s:complexType>
      <s:element name="RankingByFixtureRequest" type="tns:RankingByFixtureRequest" />
      <s:complexType name="RankingByFixtureRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="FixtureId" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfRecordsPerCategory" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="RankingByFixtureResponse" type="tns:RankingByFixtureResponse" />
      <s:complexType name="RankingByFixtureResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="RankingList" type="tns:ArrayOfRank" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="RankingBySeasonRequest" type="tns:RankingBySeasonRequest" />
      <s:complexType name="RankingBySeasonRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="SeasonId" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfRecordsPerCategory" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="RankingBySeasonResponse" type="tns:RankingBySeasonResponse" />
      <s:complexType name="RankingBySeasonResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="RankingList" type="tns:ArrayOfRank" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="RankingByRoundRequest" type="tns:RankingByRoundRequest" />
      <s:complexType name="RankingByRoundRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="RoundId" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="NumberOfRecordsPerCategory" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="RankingByRoundResponse" type="tns:RankingByRoundResponse" />
      <s:complexType name="RankingByRoundResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="RankingList" type="tns:ArrayOfRank" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:element name="JudiciaryByTeamRequest" type="tns:JudiciaryByTeamRequest" />
      <s:complexType name="JudiciaryByTeamRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="TeamId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="JudiciaryByTeamResponse" type="tns:JudiciaryByTeamResponse" />
      <s:complexType name="JudiciaryByTeamResponse">
        <s:complexContent mixed="false">
          <s:extension base="tns:BaseResponse">
            <s:sequence>
              <s:element minOccurs="0" maxOccurs="1" name="JudiciaryChargeList" type="tns:ArrayOfJudiciaryCharge" />
            </s:sequence>
          </s:extension>
        </s:complexContent>
      </s:complexType>
      <s:complexType name="ArrayOfJudiciaryCharge">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="JudiciaryCharge" nillable="true" type="tns:JudiciaryCharge" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="JudiciaryCharge">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="BasePenalty" type="s:double" />
          <s:element minOccurs="1" maxOccurs="1" name="CarryOverAdded" type="s:double" />
          <s:element minOccurs="1" maxOccurs="1" name="CarryOverRemain" type="s:double" />
          <s:element minOccurs="1" maxOccurs="1" name="EarlyPlea" type="s:double" />
          <s:element minOccurs="1" maxOccurs="1" name="FixturesPenalty" type="s:double" />
          <s:element minOccurs="1" maxOccurs="1" name="GameMinute" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="Grade" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="GuiltyPlea" type="s:double" />
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="LatePlea" type="s:double" />
          <s:element minOccurs="1" maxOccurs="1" name="OffenceId" type="s:short" />
          <s:element minOccurs="0" maxOccurs="1" name="OffenceName" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="PlayerId" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Result" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="RoundId" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="PlayerDetail" type="tns:Player" />
          <s:element minOccurs="0" maxOccurs="1" name="JudiciaryDetail" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="SubscriptionListByCodeRequest" type="tns:SubscriptionListByCodeRequest" />
      <s:complexType name="SubscriptionListByCodeRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="CodeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="SubscriptionListByCodeResponse" type="tns:SubscriptionListByCodeResponse" />
      <s:complexType name="SubscriptionListByCodeResponse">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="SubscriptionList" type="tns:ArrayOfSubscription" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="ArrayOfSubscription">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="Subscription" nillable="true" type="tns:Subscription" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="Subscription">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Name" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="TeamId" type="s:short" />
          <s:element minOccurs="1" maxOccurs="1" name="IsFree" type="s:boolean" />
        </s:sequence>
      </s:complexType>
      <s:element name="AddSubscriptionRequest" type="tns:AddSubscriptionRequest" />
      <s:complexType name="AddSubscriptionRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="SubscriptionId" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="MobileUserId" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="AddSubscriptionResponse" type="tns:AddSubscriptionResponse" />
      <s:complexType name="AddSubscriptionResponse">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="Confirmation" type="tns:Confirmation" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="Confirmation">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="OperationStatusId" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="OperationMessage" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="RemoveSubscriptionRequest" type="tns:RemoveSubscriptionRequest" />
      <s:complexType name="RemoveSubscriptionRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="SubscriptionId" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="MobileUserId" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="RemoveSubscriptionResponse" type="tns:RemoveSubscriptionResponse" />
      <s:complexType name="RemoveSubscriptionResponse">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="Confirmation" type="tns:Confirmation" />
        </s:sequence>
      </s:complexType>
      <s:element name="ClientSubscriptionsByCodeRequest" type="tns:ClientSubscriptionsByCodeRequest" />
      <s:complexType name="ClientSubscriptionsByCodeRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="CodeId" type="s:short" />
          <s:element minOccurs="0" maxOccurs="1" name="MobileUserId" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="ClientSubscriptionsByCodeResponse" type="tns:ClientSubscriptionsByCodeResponse" />
      <s:complexType name="ClientSubscriptionsByCodeResponse">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="SubscriptionList" type="tns:ArrayOfSubscription" />
        </s:sequence>
      </s:complexType>
      <s:element name="GetSportCodes">
        <s:complexType />
      </s:element>
      <s:element name="GetSportCodesResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="GetSportCodesResult" type="tns:ArrayOfSportCode" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="ArrayOfSportCode">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="SportCode" nillable="true" type="tns:SportCode" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="SportCode">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:short" />
          <s:element minOccurs="0" maxOccurs="1" name="Name" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="SportCompetitionsbyCodeRequest" type="tns:SportCompetitionsbyCodeRequest" />
      <s:complexType name="SportCompetitionsbyCodeRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="CodeId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="SportCompetitionsbyCodeResponse" type="tns:SportCompetitionsbyCodeResponse" />
      <s:complexType name="SportCompetitionsbyCodeResponse">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="CompetitionList" type="tns:ArrayOfSportCompetition" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="ArrayOfSportCompetition">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="SportCompetition" nillable="true" type="tns:SportCompetition" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="SportCompetition">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:short" />
          <s:element minOccurs="0" maxOccurs="1" name="Name" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="SportSeasonsByCompetitionRequest" type="tns:SportSeasonsByCompetitionRequest" />
      <s:complexType name="SportSeasonsByCompetitionRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="CompetitionId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="SportSeasonsByCompetitionResponse" type="tns:SportSeasonsByCompetitionResponse" />
      <s:complexType name="SportSeasonsByCompetitionResponse">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="SeasonList" type="tns:ArrayOfSportSeason" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="ArrayOfSportSeason">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="SportSeason" nillable="true" type="tns:SportSeason" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="SportSeason">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:short" />
          <s:element minOccurs="0" maxOccurs="1" name="Name" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="StartDate" type="s:dateTime" />
          <s:element minOccurs="1" maxOccurs="1" name="EndDate" type="s:dateTime" />
        </s:sequence>
      </s:complexType>
      <s:element name="CurrentSportSeasonByCompetitionRequest" type="tns:CurrentSportSeasonByCompetitionRequest" />
      <s:complexType name="CurrentSportSeasonByCompetitionRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="CompetitionId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="CurrentSportSeasonByCompetitionResponse" type="tns:CurrentSportSeasonByCompetitionResponse" />
      <s:complexType name="CurrentSportSeasonByCompetitionResponse">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="Season" type="tns:SportSeason" />
        </s:sequence>
      </s:complexType>
      <s:element name="SportRoundsBySeasonRequest" type="tns:SportRoundsBySeasonRequest" />
      <s:complexType name="SportRoundsBySeasonRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="SeasonId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="SportRoundsBySeasonResponse" type="tns:SportRoundsBySeasonResponse" />
      <s:complexType name="SportRoundsBySeasonResponse">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="RoundList" type="tns:ArrayOfSportRound" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="ArrayOfSportRound">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="SportRound" nillable="true" type="tns:SportRound" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="SportRound">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Id" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Name" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="DateDescription" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="StartDate" type="s:dateTime" />
          <s:element minOccurs="1" maxOccurs="1" name="EndDate" type="s:dateTime" />
          <s:element minOccurs="1" maxOccurs="1" name="Order" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="CurrentSportRoundBySeasonRequest" type="tns:CurrentSportRoundBySeasonRequest" />
      <s:complexType name="CurrentSportRoundBySeasonRequest">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="SeasonId" type="s:short" />
        </s:sequence>
      </s:complexType>
      <s:element name="CurrentSportRoundBySeasonResponse" type="tns:CurrentSportRoundBySeasonResponse" />
      <s:complexType name="CurrentSportRoundBySeasonResponse">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="Round" type="tns:SportRound" />
        </s:sequence>
      </s:complexType>
      <s:element name="GetLastXCommentsByFixture">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="1" maxOccurs="1" name="fixtureID" type="s:int" />
            <s:element minOccurs="1" maxOccurs="1" name="numberOfComments" type="s:int" />
            <s:element minOccurs="1" maxOccurs="1" name="applicationKey" type="s1:guid" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:element name="GetLastXCommentsByFixtureResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="GetLastXCommentsByFixtureResponse" type="tns:GetLastXCommentsByFixtureResponse" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="GetLastXCommentsByFixtureResponse">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="FixtureId" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Commentary" type="tns:ArrayOfEvent" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="ArrayOfEvent">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="Event" nillable="true" type="tns:Event" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="Event">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="Seq" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="PeriodSecs" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="Period" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Comment" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:element name="GetQuarterInfoByFixture">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="1" maxOccurs="1" name="fixtureID" type="s:int" />
            <s:element minOccurs="1" maxOccurs="1" name="applicationKey" type="s1:guid" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:element name="GetQuarterInfoByFixtureResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="GetQuarterInfoByFixtureResponse" type="tns:GetQuarterInfoByFixtureResponse" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="GetQuarterInfoByFixtureResponse">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="FixtureID" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="Quarters" type="tns:ArrayOfQuarter" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="ArrayOfQuarter">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="Quarter" nillable="true" type="tns:Quarter" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="Quarter">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="q" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="hg" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="hb" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="ag" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="ab" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="period_secs" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="period_complete" type="s1:char" />
        </s:sequence>
      </s:complexType>
      <s:element name="GetTeamStatsByFixture">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="1" maxOccurs="1" name="fixtureID" type="s:int" />
            <s:element minOccurs="1" maxOccurs="1" name="applicationKey" type="s1:guid" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:element name="GetTeamStatsByFixtureResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="GetTeamStatsByFixtureResponse" type="tns:GetTeamStatsByFixtureResponse" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="GetTeamStatsByFixtureResponse">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="FixtureID" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="IsFinal" type="s1:char" />
          <s:element minOccurs="0" maxOccurs="1" name="Stats" type="tns:ArrayOfTeamStats" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="ArrayOfTeamStats">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="TeamStats" nillable="true" type="tns:TeamStats" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="TeamStats">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="team" type="s1:char" />
          <s:element minOccurs="1" maxOccurs="1" name="k" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="h" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="m" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="ho" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="cl" nillable="true" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="ff" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="fa" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="t" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="pc" nillable="true" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="pu" nillable="true" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="edp" nillable="true" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="clg" nillable="true" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="mc" nillable="true" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="min50" nillable="true" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="in50" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="onep" nillable="true" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="bo" nillable="true" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="gap" nillable="true" type="s:int" />
        </s:sequence>
      </s:complexType>
      <s:element name="GetTeamPlayerStatsByFixture">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="1" maxOccurs="1" name="fixtureID" type="s:int" />
            <s:element minOccurs="1" maxOccurs="1" name="applicationKey" type="s1:guid" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:element name="GetTeamPlayerStatsByFixtureResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="GetTeamPlayerStatsByFixtureResponse" type="tns:GetTeamPlayerStatsByFixtureResponse" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="GetTeamPlayerStatsByFixtureResponse">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="FixtureID" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="IsFinal" type="s1:char" />
          <s:element minOccurs="0" maxOccurs="1" name="Stats" type="tns:ArrayOfPlayerStats" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="ArrayOfPlayerStats">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="unbounded" name="PlayerStats" nillable="true" type="tns:PlayerStats" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="PlayerStats">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="team" type="s1:char" />
          <s:element minOccurs="0" maxOccurs="1" name="givenName" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="familyName" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="id" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="k" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="h" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="m" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="g" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="ho" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="ff" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="fa" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="t" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="dt" type="s:int" />
        </s:sequence>
      </s:complexType>
      <s:element name="GetPlayerProfile">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="1" maxOccurs="1" name="playerID" type="s:int" />
            <s:element minOccurs="1" maxOccurs="1" name="seasonID" type="s:int" />
            <s:element minOccurs="1" maxOccurs="1" name="applicationKey" type="s1:guid" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:element name="GetPlayerProfileResponse">
        <s:complexType>
          <s:sequence>
            <s:element minOccurs="0" maxOccurs="1" name="GetPlayerProfileResponse" type="tns:GetPlayerProfileResponse" />
          </s:sequence>
        </s:complexType>
      </s:element>
      <s:complexType name="GetPlayerProfileResponse">
        <s:sequence>
          <s:element minOccurs="0" maxOccurs="1" name="player" type="tns:PlayerProfile" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="PlayerProfile">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="id" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="givenName" type="s:string" />
          <s:element minOccurs="0" maxOccurs="1" name="familyName" type="s:string" />
          <s:element minOccurs="1" maxOccurs="1" name="guernsey" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="height" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="weight" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="dob" type="s:dateTime" />
          <s:element minOccurs="1" maxOccurs="1" name="careergames" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="careergoals" type="s:int" />
          <s:element minOccurs="0" maxOccurs="1" name="seasonstats" type="tns:PlayerSeasonStats" />
          <s:element minOccurs="0" maxOccurs="1" name="bio" type="s:string" />
        </s:sequence>
      </s:complexType>
      <s:complexType name="PlayerSeasonStats">
        <s:sequence>
          <s:element minOccurs="1" maxOccurs="1" name="id" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="games" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="g" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="k" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="h" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="ff" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="fa" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="t" type="s:int" />
          <s:element minOccurs="1" maxOccurs="1" name="dt" type="s:int" />
        </s:sequence>
      </s:complexType>
    </s:schema>
    <s:schema elementFormDefault="qualified" targetNamespace="http://microsoft.com/wsdl/types/">
      <s:simpleType name="guid">
        <s:restriction base="s:string">
          <s:pattern value="[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}" />
        </s:restriction>
      </s:simpleType>
      <s:simpleType name="char">
        <s:restriction base="s:unsignedShort" />
      </s:simpleType>
    </s:schema>
  </wsdl:types>
  <wsdl:message name="GetVersionSoapIn">
    <wsdl:part name="parameters" element="tns:GetVersion" />
  </wsdl:message>
  <wsdl:message name="GetVersionSoapOut">
    <wsdl:part name="parameters" element="tns:GetVersionResponse" />
  </wsdl:message>
  <wsdl:message name="GetAnnouncementsByCompetitionSoapIn">
    <wsdl:part name="GetAnnouncementsByCompetitionRequest" element="tns:AnnouncementsByCompetitionRequest" />
  </wsdl:message>
  <wsdl:message name="GetAnnouncementsByCompetitionSoapOut">
    <wsdl:part name="GetAnnouncementsByCompetitionResult" element="tns:AnnouncementsByCompetitionResponse" />
  </wsdl:message>
  <wsdl:message name="GetAnnouncementsByTeamSoapIn">
    <wsdl:part name="GetAnnouncementsByTeamRequest" element="tns:AnnouncementsByTeamRequest" />
  </wsdl:message>
  <wsdl:message name="GetAnnouncementsByTeamSoapOut">
    <wsdl:part name="GetAnnouncementsByTeamResult" element="tns:AnnouncementsByTeamResponse" />
  </wsdl:message>
  <wsdl:message name="GetMediaProgramTypesSoapIn">
    <wsdl:part name="parameters" element="tns:GetMediaProgramTypes" />
  </wsdl:message>
  <wsdl:message name="GetMediaProgramTypesSoapOut">
    <wsdl:part name="parameters" element="tns:GetMediaProgramTypesResponse" />
  </wsdl:message>
  <wsdl:message name="GetVideosByCompetitionCSoapIn">
    <wsdl:part name="GetVideosByCompetitionRequest" element="tns:VideosByCompetitionCRequest" />
  </wsdl:message>
  <wsdl:message name="GetVideosByCompetitionCSoapOut">
    <wsdl:part name="GetVideosByCompetitionCResult" element="tns:VideosByCompetitionCResponse" />
  </wsdl:message>
  <wsdl:message name="GetVideosByCompetitionSoapIn">
    <wsdl:part name="GetVideosByCompetitionRequest" element="tns:VideosByCompetitionRequest" />
  </wsdl:message>
  <wsdl:message name="GetVideosByCompetitionSoapOut">
    <wsdl:part name="GetVideosByCompetitionResult" element="tns:VideosByCompetitionResponse" />
  </wsdl:message>
  <wsdl:message name="GetVideosByTeamCSoapIn">
    <wsdl:part name="GetVideosByTeamRequest" element="tns:VideosByTeamCRequest" />
  </wsdl:message>
  <wsdl:message name="GetVideosByTeamCSoapOut">
    <wsdl:part name="GetVideosByTeamCResult" element="tns:VideosByTeamCResponse" />
  </wsdl:message>
  <wsdl:message name="GetVideosByTeamSoapIn">
    <wsdl:part name="GetVideosByTeamRequest" element="tns:VideosByTeamRequest" />
  </wsdl:message>
  <wsdl:message name="GetVideosByTeamSoapOut">
    <wsdl:part name="GetVideosByTeamResult" element="tns:VideosByTeamResponse" />
  </wsdl:message>
  <wsdl:message name="GetVideosByFixtureCSoapIn">
    <wsdl:part name="GetVideosByFixtureRequest" element="tns:VideosByFixtureCRequest" />
  </wsdl:message>
  <wsdl:message name="GetVideosByFixtureCSoapOut">
    <wsdl:part name="GetVideosByFixtureCResult" element="tns:VideosByFixtureCResponse" />
  </wsdl:message>
  <wsdl:message name="GetVideosByFixtureSoapIn">
    <wsdl:part name="GetVideosByFixtureRequest" element="tns:VideosByFixtureRequest" />
  </wsdl:message>
  <wsdl:message name="GetVideosByFixtureSoapOut">
    <wsdl:part name="GetVideosByFixtureResult" element="tns:VideosByFixtureResponse" />
  </wsdl:message>
  <wsdl:message name="GetVideosByIdSoapIn">
    <wsdl:part name="GetVideosByIdRequest" element="tns:VideosByIdRequest" />
  </wsdl:message>
  <wsdl:message name="GetVideosByIdSoapOut">
    <wsdl:part name="GetVideosByIdResult" element="tns:VideosByIdResponse" />
  </wsdl:message>
  <wsdl:message name="GetVideosByIdCSoapIn">
    <wsdl:part name="GetVideosByIdRequest" element="tns:VideosByIdCRequest" />
  </wsdl:message>
  <wsdl:message name="GetVideosByIdCSoapOut">
    <wsdl:part name="GetVideosByIdCResult" element="tns:VideosByIdCResponse" />
  </wsdl:message>
  <wsdl:message name="GetOnlineVideosByCompetitionCSoapIn">
    <wsdl:part name="GetOnlineVideosByCompetitionRequest" element="tns:OnlineVideosByCompetitionCRequest" />
  </wsdl:message>
  <wsdl:message name="GetOnlineVideosByCompetitionCSoapOut">
    <wsdl:part name="GetOnlineVideosByCompetitionCResult" element="tns:OnlineVideosByCompetitionResponse" />
  </wsdl:message>
  <wsdl:message name="GetOnlineVideosByCompetitionSoapIn">
    <wsdl:part name="GetOnlineVideosByCompetitionRequest" element="tns:OnlineVideosByCompetitionRequest" />
  </wsdl:message>
  <wsdl:message name="GetOnlineVideosByCompetitionSoapOut">
    <wsdl:part name="GetOnlineVideosByCompetitionResult" element="tns:OnlineVideosByCompetitionResponse" />
  </wsdl:message>
  <wsdl:message name="GetOnlineVideosByTeamCSoapIn">
    <wsdl:part name="GetOnlineVideosByTeamRequest" element="tns:OnlineVideosByTeamCRequest" />
  </wsdl:message>
  <wsdl:message name="GetOnlineVideosByTeamCSoapOut">
    <wsdl:part name="GetOnlineVideosByTeamCResult" element="tns:OnlineVideosByTeamResponse" />
  </wsdl:message>
  <wsdl:message name="GetOnlineVideosByTeamSoapIn">
    <wsdl:part name="GetOnlineVideosByTeamRequest" element="tns:OnlineVideosByTeamRequest" />
  </wsdl:message>
  <wsdl:message name="GetOnlineVideosByTeamSoapOut">
    <wsdl:part name="GetOnlineVideosByTeamResult" element="tns:OnlineVideosByTeamResponse" />
  </wsdl:message>
  <wsdl:message name="GetOnlineVideosByFixtureCSoapIn">
    <wsdl:part name="GetOnlineVideosByFixtureRequest" element="tns:OnlineVideosByFixtureCRequest" />
  </wsdl:message>
  <wsdl:message name="GetOnlineVideosByFixtureCSoapOut">
    <wsdl:part name="GetOnlineVideosByFixtureCResult" element="tns:OnlineVideosByFixtureResponse" />
  </wsdl:message>
  <wsdl:message name="GetOnlineVideosByFixtureSoapIn">
    <wsdl:part name="GetOnlineVideosByFixtureRequest" element="tns:OnlineVideosByFixtureRequest" />
  </wsdl:message>
  <wsdl:message name="GetOnlineVideosByFixtureSoapOut">
    <wsdl:part name="GetOnlineVideosByFixtureResult" element="tns:OnlineVideosByFixtureResponse" />
  </wsdl:message>
  <wsdl:message name="GetOnlineVideosByIdSoapIn">
    <wsdl:part name="parameters" element="tns:GetOnlineVideosById" />
  </wsdl:message>
  <wsdl:message name="GetOnlineVideosByIdSoapOut">
    <wsdl:part name="parameters" element="tns:GetOnlineVideosByIdResponse" />
  </wsdl:message>
  <wsdl:message name="GetRelatedVideosByVideoSoapIn">
    <wsdl:part name="GetRelatedVideosByVideoRequest" element="tns:RelatedVideosByVideoRequest" />
  </wsdl:message>
  <wsdl:message name="GetRelatedVideosByVideoSoapOut">
    <wsdl:part name="GetRelatedVideosByVideoResult" element="tns:RelatedVideosByVideoResponse" />
  </wsdl:message>
  <wsdl:message name="GetNewsCategoriesSoapIn">
    <wsdl:part name="parameters" element="tns:GetNewsCategories" />
  </wsdl:message>
  <wsdl:message name="GetNewsCategoriesSoapOut">
    <wsdl:part name="parameters" element="tns:GetNewsCategoriesResponse" />
  </wsdl:message>
  <wsdl:message name="GetNewsArticlesByCompetitionSoapIn">
    <wsdl:part name="GetNewsArticlesByCompetitionRequest" element="tns:NewsArticleByCompetitionRequest" />
  </wsdl:message>
  <wsdl:message name="GetNewsArticlesByCompetitionSoapOut">
    <wsdl:part name="GetNewsArticlesByCompetitionResult" element="tns:NewsArticleByCompetitionResponse" />
  </wsdl:message>
  <wsdl:message name="GetNewsArticlesByTeamSoapIn">
    <wsdl:part name="GetNewsArticlesByTeamRequest" element="tns:NewsArticleByTeamRequest" />
  </wsdl:message>
  <wsdl:message name="GetNewsArticlesByTeamSoapOut">
    <wsdl:part name="GetNewsArticlesByTeamResult" element="tns:NewsArticleByTeamResponse" />
  </wsdl:message>
  <wsdl:message name="GetNewsArticlesByFixtureSoapIn">
    <wsdl:part name="GetNewsArticlesByFixtureRequest" element="tns:NewsArticlesByFixtureRequest" />
  </wsdl:message>
  <wsdl:message name="GetNewsArticlesByFixtureSoapOut">
    <wsdl:part name="GetNewsArticlesByFixtureResult" element="tns:NewsArticlesByFixtureResponse" />
  </wsdl:message>
  <wsdl:message name="GetNewsArticleByIdSoapIn">
    <wsdl:part name="GetNewsArticleByIdRequest" element="tns:NewsArticleByIdRequest" />
  </wsdl:message>
  <wsdl:message name="GetNewsArticleByIdSoapOut">
    <wsdl:part name="GetNewsArticleByIdResult" element="tns:NewsArticleByIdResponse" />
  </wsdl:message>
  <wsdl:message name="GetRelatedNewsArticlesByNewsSoapIn">
    <wsdl:part name="GetRelatedNewsArticlesByNewsRequest" element="tns:RelatedNewsArticlesByNewsRequest" />
  </wsdl:message>
  <wsdl:message name="GetRelatedNewsArticlesByNewsSoapOut">
    <wsdl:part name="GetRelatedNewsArticlesByNewsResult" element="tns:RelatedNewsArticlesByNewsResponse" />
  </wsdl:message>
  <wsdl:message name="GetCrossProductLinksByCompetitionSoapIn">
    <wsdl:part name="GetCrossProductLinksByCompetitionRequest" element="tns:CrossProductLinksByCompetitionRequest" />
  </wsdl:message>
  <wsdl:message name="GetCrossProductLinksByCompetitionSoapOut">
    <wsdl:part name="GetCrossProductLinksByCompetitionResult" element="tns:CrossProductLinksByCompetitionResponse" />
  </wsdl:message>
  <wsdl:message name="GetLadderBySeasonSoapIn">
    <wsdl:part name="GetLadderBySeasonRequest" element="tns:LadderRequest" />
  </wsdl:message>
  <wsdl:message name="GetLadderBySeasonSoapOut">
    <wsdl:part name="GetLadderBySeasonResult" element="tns:LadderResponse" />
  </wsdl:message>
  <wsdl:message name="GetTeamsBySeasonSoapIn">
    <wsdl:part name="GetTeamsBySeasonRequest" element="tns:TeamsRequest" />
  </wsdl:message>
  <wsdl:message name="GetTeamsBySeasonSoapOut">
    <wsdl:part name="GetTeamsBySeasonResult" element="tns:TeamsResponse" />
  </wsdl:message>
  <wsdl:message name="GetPlayersByTeamSoapIn">
    <wsdl:part name="GetPlayersByTeamRequest" element="tns:PlayersByTeamRequest" />
  </wsdl:message>
  <wsdl:message name="GetPlayersByTeamSoapOut">
    <wsdl:part name="GetPlayersByTeamResult" element="tns:PlayersByTeamResponse" />
  </wsdl:message>
  <wsdl:message name="GetPlayersByTeamAndFixtureSoapIn">
    <wsdl:part name="GetPlayersByTeamAndFixtureRequest" element="tns:PlayersByTeamAndFixtureRequest" />
  </wsdl:message>
  <wsdl:message name="GetPlayersByTeamAndFixtureSoapOut">
    <wsdl:part name="GetPlayersByTeamAndFixtureResult" element="tns:PlayersByTeamAndFixtureResponse" />
  </wsdl:message>
  <wsdl:message name="GetFixturesByRoundSoapIn">
    <wsdl:part name="GetFixturesByRoundRequest" element="tns:FixturesByRoundRequest" />
  </wsdl:message>
  <wsdl:message name="GetFixturesByRoundSoapOut">
    <wsdl:part name="GetFixturesByRoundResult" element="tns:FixturesByRoundResponse" />
  </wsdl:message>
  <wsdl:message name="GetFixturesBySeasonSoapIn">
    <wsdl:part name="GetFixturesBySeasonRequest" element="tns:FixturesbySeasonRequest" />
  </wsdl:message>
  <wsdl:message name="GetFixturesBySeasonSoapOut">
    <wsdl:part name="GetFixturesBySeasonResult" element="tns:FixturesbySeasonResponse" />
  </wsdl:message>
  <wsdl:message name="GetFixturesBySeasonAndTeamSoapIn">
    <wsdl:part name="GetFixturesBySeasonAndTeamRequest" element="tns:FixturesBySeasonAndTeamRequest" />
  </wsdl:message>
  <wsdl:message name="GetFixturesBySeasonAndTeamSoapOut">
    <wsdl:part name="GetFixturesBySeasonAndTeamResult" element="tns:FixturesBySeasonAndTeamResponse" />
  </wsdl:message>
  <wsdl:message name="GetFixtureByIdSoapIn">
    <wsdl:part name="GetFixtureByIdRequest" element="tns:FixtureByIdRequest" />
  </wsdl:message>
  <wsdl:message name="GetFixtureByIdSoapOut">
    <wsdl:part name="GetFixtureByIdResult" element="tns:FixtureByIdResponse" />
  </wsdl:message>
  <wsdl:message name="GetRankingsByTeamSoapIn">
    <wsdl:part name="GetRankingsByTeamRequest" element="tns:RankingByTeamRequest" />
  </wsdl:message>
  <wsdl:message name="GetRankingsByTeamSoapOut">
    <wsdl:part name="GetRankingsByTeamResult" element="tns:RankingByTeamResponse" />
  </wsdl:message>
  <wsdl:message name="GetRankingsByFixtureSoapIn">
    <wsdl:part name="GetRankingsByFixtureRequest" element="tns:RankingByFixtureRequest" />
  </wsdl:message>
  <wsdl:message name="GetRankingsByFixtureSoapOut">
    <wsdl:part name="GetRankingsByFixtureResult" element="tns:RankingByFixtureResponse" />
  </wsdl:message>
  <wsdl:message name="GetRankingsBySeasonSoapIn">
    <wsdl:part name="GetRankingsBySeasonRequest" element="tns:RankingBySeasonRequest" />
  </wsdl:message>
  <wsdl:message name="GetRankingsBySeasonSoapOut">
    <wsdl:part name="GetRankingsBySeasonResult" element="tns:RankingBySeasonResponse" />
  </wsdl:message>
  <wsdl:message name="GetRankingsByRoundSoapIn">
    <wsdl:part name="GetRankingsByRoundRequest" element="tns:RankingByRoundRequest" />
  </wsdl:message>
  <wsdl:message name="GetRankingsByRoundSoapOut">
    <wsdl:part name="GetRankingsByRoundResult" element="tns:RankingByRoundResponse" />
  </wsdl:message>
  <wsdl:message name="GetJudiciaryByTeamSoapIn">
    <wsdl:part name="GetJudiciaryByTeamRequest" element="tns:JudiciaryByTeamRequest" />
  </wsdl:message>
  <wsdl:message name="GetJudiciaryByTeamSoapOut">
    <wsdl:part name="GetJudiciaryByTeamResult" element="tns:JudiciaryByTeamResponse" />
  </wsdl:message>
  <wsdl:message name="GetSubscriptionListByCodeSoapIn">
    <wsdl:part name="GetSubscriptionListByCodeRequest" element="tns:SubscriptionListByCodeRequest" />
  </wsdl:message>
  <wsdl:message name="GetSubscriptionListByCodeSoapOut">
    <wsdl:part name="GetSubscriptionListByCodeResult" element="tns:SubscriptionListByCodeResponse" />
  </wsdl:message>
  <wsdl:message name="AddSubscriptionSoapIn">
    <wsdl:part name="AddSubscriptionRequest" element="tns:AddSubscriptionRequest" />
  </wsdl:message>
  <wsdl:message name="AddSubscriptionSoapOut">
    <wsdl:part name="AddSubscriptionResult" element="tns:AddSubscriptionResponse" />
  </wsdl:message>
  <wsdl:message name="RemoveSubscriptionSoapIn">
    <wsdl:part name="RemoveSubscriptionRequest" element="tns:RemoveSubscriptionRequest" />
  </wsdl:message>
  <wsdl:message name="RemoveSubscriptionSoapOut">
    <wsdl:part name="RemoveSubscriptionResult" element="tns:RemoveSubscriptionResponse" />
  </wsdl:message>
  <wsdl:message name="GetClientSubscriptionsByCodeSoapIn">
    <wsdl:part name="GetClientSubscriptionsByCodeRequest" element="tns:ClientSubscriptionsByCodeRequest" />
  </wsdl:message>
  <wsdl:message name="GetClientSubscriptionsByCodeSoapOut">
    <wsdl:part name="GetClientSubscriptionsByCodeResult" element="tns:ClientSubscriptionsByCodeResponse" />
  </wsdl:message>
  <wsdl:message name="GetSportCodesSoapIn">
    <wsdl:part name="parameters" element="tns:GetSportCodes" />
  </wsdl:message>
  <wsdl:message name="GetSportCodesSoapOut">
    <wsdl:part name="parameters" element="tns:GetSportCodesResponse" />
  </wsdl:message>
  <wsdl:message name="GetSportCompetitionsByCodeSoapIn">
    <wsdl:part name="GetSportCompetitionsByCodeRequest" element="tns:SportCompetitionsbyCodeRequest" />
  </wsdl:message>
  <wsdl:message name="GetSportCompetitionsByCodeSoapOut">
    <wsdl:part name="GetSportCompetitionsByCodeResult" element="tns:SportCompetitionsbyCodeResponse" />
  </wsdl:message>
  <wsdl:message name="GetSportSeasonsByCompetitionSoapIn">
    <wsdl:part name="GetSportSeasonsByCompetitionRequest" element="tns:SportSeasonsByCompetitionRequest" />
  </wsdl:message>
  <wsdl:message name="GetSportSeasonsByCompetitionSoapOut">
    <wsdl:part name="GetSportSeasonsByCompetitionResult" element="tns:SportSeasonsByCompetitionResponse" />
  </wsdl:message>
  <wsdl:message name="GetCurrentSportSeasonByCompetitionSoapIn">
    <wsdl:part name="GetCurrentSportSeasonByCompetitionRequest" element="tns:CurrentSportSeasonByCompetitionRequest" />
  </wsdl:message>
  <wsdl:message name="GetCurrentSportSeasonByCompetitionSoapOut">
    <wsdl:part name="GetCurrentSportSeasonByCompetitionResult" element="tns:CurrentSportSeasonByCompetitionResponse" />
  </wsdl:message>
  <wsdl:message name="GetSportRoundsBySeasonSoapIn">
    <wsdl:part name="GetSportRoundsBySeasonRequest" element="tns:SportRoundsBySeasonRequest" />
  </wsdl:message>
  <wsdl:message name="GetSportRoundsBySeasonSoapOut">
    <wsdl:part name="GetSportRoundsBySeasonResult" element="tns:SportRoundsBySeasonResponse" />
  </wsdl:message>
  <wsdl:message name="GetCurrentSportRoundBySeasonSoapIn">
    <wsdl:part name="GetCurrentSportRoundBySeasonRequest" element="tns:CurrentSportRoundBySeasonRequest" />
  </wsdl:message>
  <wsdl:message name="GetCurrentSportRoundBySeasonSoapOut">
    <wsdl:part name="GetCurrentSportRoundBySeasonResult" element="tns:CurrentSportRoundBySeasonResponse" />
  </wsdl:message>
  <wsdl:message name="GetLastXCommentsByFixtureSoapIn">
    <wsdl:part name="parameters" element="tns:GetLastXCommentsByFixture" />
  </wsdl:message>
  <wsdl:message name="GetLastXCommentsByFixtureSoapOut">
    <wsdl:part name="parameters" element="tns:GetLastXCommentsByFixtureResponse" />
  </wsdl:message>
  <wsdl:message name="GetQuarterInfoByFixtureSoapIn">
    <wsdl:part name="parameters" element="tns:GetQuarterInfoByFixture" />
  </wsdl:message>
  <wsdl:message name="GetQuarterInfoByFixtureSoapOut">
    <wsdl:part name="parameters" element="tns:GetQuarterInfoByFixtureResponse" />
  </wsdl:message>
  <wsdl:message name="GetTeamStatsByFixtureSoapIn">
    <wsdl:part name="parameters" element="tns:GetTeamStatsByFixture" />
  </wsdl:message>
  <wsdl:message name="GetTeamStatsByFixtureSoapOut">
    <wsdl:part name="parameters" element="tns:GetTeamStatsByFixtureResponse" />
  </wsdl:message>
  <wsdl:message name="GetTeamPlayerStatsByFixtureSoapIn">
    <wsdl:part name="parameters" element="tns:GetTeamPlayerStatsByFixture" />
  </wsdl:message>
  <wsdl:message name="GetTeamPlayerStatsByFixtureSoapOut">
    <wsdl:part name="parameters" element="tns:GetTeamPlayerStatsByFixtureResponse" />
  </wsdl:message>
  <wsdl:message name="GetPlayerProfileSoapIn">
    <wsdl:part name="parameters" element="tns:GetPlayerProfile" />
  </wsdl:message>
  <wsdl:message name="GetPlayerProfileSoapOut">
    <wsdl:part name="parameters" element="tns:GetPlayerProfileResponse" />
  </wsdl:message>
  <wsdl:portType name="MobileWebServices">
    <wsdl:operation name="GetVersion">
      <wsdl:input message="tns:GetVersionSoapIn" />
      <wsdl:output message="tns:GetVersionSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetAnnouncementsByCompetition">
      <wsdl:input message="tns:GetAnnouncementsByCompetitionSoapIn" />
      <wsdl:output message="tns:GetAnnouncementsByCompetitionSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetAnnouncementsByTeam">
      <wsdl:input message="tns:GetAnnouncementsByTeamSoapIn" />
      <wsdl:output message="tns:GetAnnouncementsByTeamSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetMediaProgramTypes">
      <wsdl:input message="tns:GetMediaProgramTypesSoapIn" />
      <wsdl:output message="tns:GetMediaProgramTypesSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetVideosByCompetitionC">
      <wsdl:input message="tns:GetVideosByCompetitionCSoapIn" />
      <wsdl:output message="tns:GetVideosByCompetitionCSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetVideosByCompetition">
      <wsdl:input message="tns:GetVideosByCompetitionSoapIn" />
      <wsdl:output message="tns:GetVideosByCompetitionSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetVideosByTeamC">
      <wsdl:input message="tns:GetVideosByTeamCSoapIn" />
      <wsdl:output message="tns:GetVideosByTeamCSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetVideosByTeam">
      <wsdl:input message="tns:GetVideosByTeamSoapIn" />
      <wsdl:output message="tns:GetVideosByTeamSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetVideosByFixtureC">
      <wsdl:input message="tns:GetVideosByFixtureCSoapIn" />
      <wsdl:output message="tns:GetVideosByFixtureCSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetVideosByFixture">
      <wsdl:input message="tns:GetVideosByFixtureSoapIn" />
      <wsdl:output message="tns:GetVideosByFixtureSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetVideosById">
      <wsdl:input message="tns:GetVideosByIdSoapIn" />
      <wsdl:output message="tns:GetVideosByIdSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetVideosByIdC">
      <wsdl:input message="tns:GetVideosByIdCSoapIn" />
      <wsdl:output message="tns:GetVideosByIdCSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetOnlineVideosByCompetitionC">
      <wsdl:input message="tns:GetOnlineVideosByCompetitionCSoapIn" />
      <wsdl:output message="tns:GetOnlineVideosByCompetitionCSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetOnlineVideosByCompetition">
      <wsdl:input message="tns:GetOnlineVideosByCompetitionSoapIn" />
      <wsdl:output message="tns:GetOnlineVideosByCompetitionSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetOnlineVideosByTeamC">
      <wsdl:input message="tns:GetOnlineVideosByTeamCSoapIn" />
      <wsdl:output message="tns:GetOnlineVideosByTeamCSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetOnlineVideosByTeam">
      <wsdl:input message="tns:GetOnlineVideosByTeamSoapIn" />
      <wsdl:output message="tns:GetOnlineVideosByTeamSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetOnlineVideosByFixtureC">
      <wsdl:input message="tns:GetOnlineVideosByFixtureCSoapIn" />
      <wsdl:output message="tns:GetOnlineVideosByFixtureCSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetOnlineVideosByFixture">
      <wsdl:input message="tns:GetOnlineVideosByFixtureSoapIn" />
      <wsdl:output message="tns:GetOnlineVideosByFixtureSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetOnlineVideosById">
      <wsdl:input message="tns:GetOnlineVideosByIdSoapIn" />
      <wsdl:output message="tns:GetOnlineVideosByIdSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetRelatedVideosByVideo">
      <wsdl:input message="tns:GetRelatedVideosByVideoSoapIn" />
      <wsdl:output message="tns:GetRelatedVideosByVideoSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetNewsCategories">
      <wsdl:input message="tns:GetNewsCategoriesSoapIn" />
      <wsdl:output message="tns:GetNewsCategoriesSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetNewsArticlesByCompetition">
      <wsdl:input message="tns:GetNewsArticlesByCompetitionSoapIn" />
      <wsdl:output message="tns:GetNewsArticlesByCompetitionSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetNewsArticlesByTeam">
      <wsdl:input message="tns:GetNewsArticlesByTeamSoapIn" />
      <wsdl:output message="tns:GetNewsArticlesByTeamSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetNewsArticlesByFixture">
      <wsdl:input message="tns:GetNewsArticlesByFixtureSoapIn" />
      <wsdl:output message="tns:GetNewsArticlesByFixtureSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetNewsArticleById">
      <wsdl:input message="tns:GetNewsArticleByIdSoapIn" />
      <wsdl:output message="tns:GetNewsArticleByIdSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetRelatedNewsArticlesByNews">
      <wsdl:input message="tns:GetRelatedNewsArticlesByNewsSoapIn" />
      <wsdl:output message="tns:GetRelatedNewsArticlesByNewsSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetCrossProductLinksByCompetition">
      <wsdl:input message="tns:GetCrossProductLinksByCompetitionSoapIn" />
      <wsdl:output message="tns:GetCrossProductLinksByCompetitionSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetLadderBySeason">
      <wsdl:input message="tns:GetLadderBySeasonSoapIn" />
      <wsdl:output message="tns:GetLadderBySeasonSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetTeamsBySeason">
      <wsdl:input message="tns:GetTeamsBySeasonSoapIn" />
      <wsdl:output message="tns:GetTeamsBySeasonSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetPlayersByTeam">
      <wsdl:input message="tns:GetPlayersByTeamSoapIn" />
      <wsdl:output message="tns:GetPlayersByTeamSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetPlayersByTeamAndFixture">
      <wsdl:input message="tns:GetPlayersByTeamAndFixtureSoapIn" />
      <wsdl:output message="tns:GetPlayersByTeamAndFixtureSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetFixturesByRound">
      <wsdl:input message="tns:GetFixturesByRoundSoapIn" />
      <wsdl:output message="tns:GetFixturesByRoundSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetFixturesBySeason">
      <wsdl:input message="tns:GetFixturesBySeasonSoapIn" />
      <wsdl:output message="tns:GetFixturesBySeasonSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetFixturesBySeasonAndTeam">
      <wsdl:input message="tns:GetFixturesBySeasonAndTeamSoapIn" />
      <wsdl:output message="tns:GetFixturesBySeasonAndTeamSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetFixtureById">
      <wsdl:input message="tns:GetFixtureByIdSoapIn" />
      <wsdl:output message="tns:GetFixtureByIdSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetRankingsByTeam">
      <wsdl:input message="tns:GetRankingsByTeamSoapIn" />
      <wsdl:output message="tns:GetRankingsByTeamSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetRankingsByFixture">
      <wsdl:input message="tns:GetRankingsByFixtureSoapIn" />
      <wsdl:output message="tns:GetRankingsByFixtureSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetRankingsBySeason">
      <wsdl:input message="tns:GetRankingsBySeasonSoapIn" />
      <wsdl:output message="tns:GetRankingsBySeasonSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetRankingsByRound">
      <wsdl:input message="tns:GetRankingsByRoundSoapIn" />
      <wsdl:output message="tns:GetRankingsByRoundSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetJudiciaryByTeam">
      <wsdl:input message="tns:GetJudiciaryByTeamSoapIn" />
      <wsdl:output message="tns:GetJudiciaryByTeamSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetSubscriptionListByCode">
      <wsdl:input message="tns:GetSubscriptionListByCodeSoapIn" />
      <wsdl:output message="tns:GetSubscriptionListByCodeSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="AddSubscription">
      <wsdl:input message="tns:AddSubscriptionSoapIn" />
      <wsdl:output message="tns:AddSubscriptionSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="RemoveSubscription">
      <wsdl:input message="tns:RemoveSubscriptionSoapIn" />
      <wsdl:output message="tns:RemoveSubscriptionSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetClientSubscriptionsByCode">
      <wsdl:input message="tns:GetClientSubscriptionsByCodeSoapIn" />
      <wsdl:output message="tns:GetClientSubscriptionsByCodeSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetSportCodes">
      <wsdl:input message="tns:GetSportCodesSoapIn" />
      <wsdl:output message="tns:GetSportCodesSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetSportCompetitionsByCode">
      <wsdl:input message="tns:GetSportCompetitionsByCodeSoapIn" />
      <wsdl:output message="tns:GetSportCompetitionsByCodeSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetSportSeasonsByCompetition">
      <wsdl:input message="tns:GetSportSeasonsByCompetitionSoapIn" />
      <wsdl:output message="tns:GetSportSeasonsByCompetitionSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetCurrentSportSeasonByCompetition">
      <wsdl:input message="tns:GetCurrentSportSeasonByCompetitionSoapIn" />
      <wsdl:output message="tns:GetCurrentSportSeasonByCompetitionSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetSportRoundsBySeason">
      <wsdl:input message="tns:GetSportRoundsBySeasonSoapIn" />
      <wsdl:output message="tns:GetSportRoundsBySeasonSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetCurrentSportRoundBySeason">
      <wsdl:input message="tns:GetCurrentSportRoundBySeasonSoapIn" />
      <wsdl:output message="tns:GetCurrentSportRoundBySeasonSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetLastXCommentsByFixture">
      <wsdl:input message="tns:GetLastXCommentsByFixtureSoapIn" />
      <wsdl:output message="tns:GetLastXCommentsByFixtureSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetQuarterInfoByFixture">
      <wsdl:input message="tns:GetQuarterInfoByFixtureSoapIn" />
      <wsdl:output message="tns:GetQuarterInfoByFixtureSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetTeamStatsByFixture">
      <wsdl:input message="tns:GetTeamStatsByFixtureSoapIn" />
      <wsdl:output message="tns:GetTeamStatsByFixtureSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetTeamPlayerStatsByFixture">
      <wsdl:input message="tns:GetTeamPlayerStatsByFixtureSoapIn" />
      <wsdl:output message="tns:GetTeamPlayerStatsByFixtureSoapOut" />
    </wsdl:operation>
    <wsdl:operation name="GetPlayerProfile">
      <wsdl:input message="tns:GetPlayerProfileSoapIn" />
      <wsdl:output message="tns:GetPlayerProfileSoapOut" />
    </wsdl:operation>
  </wsdl:portType>
  <wsdl:binding name="MobileWebServices" type="tns:MobileWebServices">
    <wsdl:documentation>
      <wsi:Claim conformsTo="http://ws-i.org/profiles/basic/1.1" xmlns:wsi="http://ws-i.org/schemas/conformanceClaim/" />
    </wsdl:documentation>
    <soap:binding transport="http://schemas.xmlsoap.org/soap/http" />
    <wsdl:operation name="GetVersion">
      <soap:operation soapAction="http://xml.afl.com.au/GetVersion" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetAnnouncementsByCompetition">
      <soap:operation soapAction="http://xml.afl.com.au/GetAnnouncementsByCompetition" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetAnnouncementsByTeam">
      <soap:operation soapAction="http://xml.afl.com.au/GetAnnouncementsByTeam" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetMediaProgramTypes">
      <soap:operation soapAction="http://xml.afl.com.au/GetMediaProgramTypes" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetVideosByCompetitionC">
      <soap:operation soapAction="http://xml.afl.com.au/GetVideosByCompetitionC" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetVideosByCompetition">
      <soap:operation soapAction="http://xml.afl.com.au/GetVideosByCompetition" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetVideosByTeamC">
      <soap:operation soapAction="http://xml.afl.com.au/GetVideosByTeamC" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetVideosByTeam">
      <soap:operation soapAction="http://xml.afl.com.au/GetVideosByTeam" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetVideosByFixtureC">
      <soap:operation soapAction="http://xml.afl.com.au/GetVideosByFixtureC" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetVideosByFixture">
      <soap:operation soapAction="http://xml.afl.com.au/GetVideosByFixture" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetVideosById">
      <soap:operation soapAction="http://xml.afl.com.au/GetVideosById" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetVideosByIdC">
      <soap:operation soapAction="http://xml.afl.com.au/GetVideosByIdC" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetOnlineVideosByCompetitionC">
      <soap:operation soapAction="http://xml.afl.com.au/GetOnlineVideosByCompetitionC" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetOnlineVideosByCompetition">
      <soap:operation soapAction="http://xml.afl.com.au/GetOnlineVideosByCompetition" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetOnlineVideosByTeamC">
      <soap:operation soapAction="http://xml.afl.com.au/GetOnlineVideosByTeamC" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetOnlineVideosByTeam">
      <soap:operation soapAction="http://xml.afl.com.au/GetOnlineVideosByTeam" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetOnlineVideosByFixtureC">
      <soap:operation soapAction="http://xml.afl.com.au/GetOnlineVideosByFixtureC" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetOnlineVideosByFixture">
      <soap:operation soapAction="http://xml.afl.com.au/GetOnlineVideosByFixture" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetOnlineVideosById">
      <soap:operation soapAction="http://xml.afl.com.au/GetOnlineVideosById" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetRelatedVideosByVideo">
      <soap:operation soapAction="http://xml.afl.com.au/GetRelatedVideosByVideo" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetNewsCategories">
      <soap:operation soapAction="http://xml.afl.com.au/GetNewsCategories" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetNewsArticlesByCompetition">
      <soap:operation soapAction="http://xml.afl.com.au/GetNewsArticlesByCompetition" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetNewsArticlesByTeam">
      <soap:operation soapAction="http://xml.afl.com.au/GetNewsArticlesByTeam" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetNewsArticlesByFixture">
      <soap:operation soapAction="http://xml.afl.com.au/GetNewsArticlesByFixture" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetNewsArticleById">
      <soap:operation soapAction="http://xml.afl.com.au/GetNewsArticleById" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetRelatedNewsArticlesByNews">
      <soap:operation soapAction="http://xml.afl.com.au/GetRelatedNewsArticlesByNews" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetCrossProductLinksByCompetition">
      <soap:operation soapAction="http://xml.afl.com.au/GetCrossProductLinksByCompetition" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetLadderBySeason">
      <soap:operation soapAction="http://xml.afl.com.au/GetLadderBySeason" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetTeamsBySeason">
      <soap:operation soapAction="http://xml.afl.com.au/GetTeamsBySeason" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetPlayersByTeam">
      <soap:operation soapAction="http://xml.afl.com.au/GetPlayersByTeam" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetPlayersByTeamAndFixture">
      <soap:operation soapAction="http://xml.afl.com.au/GetPlayersByTeamAndFixture" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetFixturesByRound">
      <soap:operation soapAction="http://xml.afl.com.au/GetFixturesByRound" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetFixturesBySeason">
      <soap:operation soapAction="http://xml.afl.com.au/GetFixturesBySeason" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetFixturesBySeasonAndTeam">
      <soap:operation soapAction="http://xml.afl.com.au/GetFixturesBySeasonAndTeam" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetFixtureById">
      <soap:operation soapAction="http://xml.afl.com.au/GetFixtureById" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetRankingsByTeam">
      <soap:operation soapAction="http://xml.afl.com.au/GetRankingsByTeam" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetRankingsByFixture">
      <soap:operation soapAction="http://xml.afl.com.au/GetRankingsByFixture" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetRankingsBySeason">
      <soap:operation soapAction="http://xml.afl.com.au/GetRankingsBySeason" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetRankingsByRound">
      <soap:operation soapAction="http://xml.afl.com.au/GetRankingsByRound" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetJudiciaryByTeam">
      <soap:operation soapAction="http://xml.afl.com.au/GetJudiciaryByTeam" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetSubscriptionListByCode">
      <soap:operation soapAction="http://xml.afl.com.au/GetSubscriptionListByCode" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="AddSubscription">
      <soap:operation soapAction="http://xml.afl.com.au/AddSubscription" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="RemoveSubscription">
      <soap:operation soapAction="http://xml.afl.com.au/RemoveSubscription" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetClientSubscriptionsByCode">
      <soap:operation soapAction="http://xml.afl.com.au/GetClientSubscriptionsByCode" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetSportCodes">
      <soap:operation soapAction="http://xml.afl.com.au/GetSportCodes" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetSportCompetitionsByCode">
      <soap:operation soapAction="http://xml.afl.com.au/GetSportCompetitionsByCode" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetSportSeasonsByCompetition">
      <soap:operation soapAction="http://xml.afl.com.au/GetSportSeasonsByCompetition" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetCurrentSportSeasonByCompetition">
      <soap:operation soapAction="http://xml.afl.com.au/GetCurrentSportSeasonByCompetition" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetSportRoundsBySeason">
      <soap:operation soapAction="http://xml.afl.com.au/GetSportRoundsBySeason" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetCurrentSportRoundBySeason">
      <soap:operation soapAction="http://xml.afl.com.au/GetCurrentSportRoundBySeason" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetLastXCommentsByFixture">
      <soap:operation soapAction="http://xml.afl.com.au/GetLastXCommentsByFixture" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetQuarterInfoByFixture">
      <soap:operation soapAction="http://xml.afl.com.au/GetQuarterInfoByFixture" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetTeamStatsByFixture">
      <soap:operation soapAction="http://xml.afl.com.au/GetTeamStatsByFixture" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetTeamPlayerStatsByFixture">
      <soap:operation soapAction="http://xml.afl.com.au/GetTeamPlayerStatsByFixture" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
    <wsdl:operation name="GetPlayerProfile">
      <soap:operation soapAction="http://xml.afl.com.au/GetPlayerProfile" style="document" />
      <wsdl:input>
        <soap:body use="literal" />
      </wsdl:input>
      <wsdl:output>
        <soap:body use="literal" />
      </wsdl:output>
    </wsdl:operation>
  </wsdl:binding>
  <wsdl:service name="MobileWebServices">
    <wsdl:port name="MobileWebServices" binding="tns:MobileWebServices">
      <soap:address location="http://xml.afl.com.au/mobilewebservices.asmx" />
    </wsdl:port>
  </wsdl:service>
</wsdl:definitions>