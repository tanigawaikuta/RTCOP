//================================================================================
// EnglishLayer.cpp
//
// 役割: EnglishLayerを表すクラス。自動生成。
//================================================================================

#include "RTCOP_Generated/API.h"
#include "RTCOP_Generated/EnglishLayer.h"
#include "RTCOP_Generated/Layer_Private.h"
#include "RTCOP_Generated/LayerdObject_Private.h"
#include "RTCOP/Framework.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "RTCOP_Generated/DependentCode/ExecuteProceed.h"
#include "RTCOP_Generated/DependentCode/GetVirtualFunctionTable.h"
#include "RTCOP_Generated/DependentCode/GetLayerdObjectFinalizer.h"
#include <iostream>
#include <cstring>

#include "Hello.h"
#include <stdio.h>

namespace RTCOP {
namespace Generated {

//--------------------------------------------------------------
// レイヤの実装
//--------------------------------------------------------------
// インスタンスの取得
EnglishLayer* EnglishLayer::GetInstance()
{
	return (EnglishLayer*)RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer((int)LayerID::EnglishLayer);
}

// コンストラクタ
EnglishLayer::EnglishLayer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)
	:RTCOP::Core::Layer(id, name, numOfBaseClasses, numOfBaseMethods)
{
	// Helloクラスの仮想関数テーブル
	int size0 = sizeof(volatile void*) * numOfBaseMethods[0];
	volatile void** virtualFunctionTable0 = DependentCode::HelloClass::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[0], virtualFunctionTable0, size0);
	// 再定義なしのメソッドやデストラクタは0にしておく
	_Private->_VirtualFunctionTables[0][1] = 0;
	_Private->_VirtualFunctionTables[0][2] = 0;
	_Private->_VirtualFunctionTables[0][3] = 0;
#if defined(LINUX_X86) || defined(LINUX_X64) || defined(LINUX_ARM) || defined(LINUX_ARM64) || defined(WIN32_MINGW) || defined(WIN64_MINGW) || defined(MACOS_X64)
	_Private->_VirtualFunctionTables[0][4] = 0;
#endif
}

// デストラクタ
EnglishLayer::~EnglishLayer()
{
}

// レイヤードオブジェクトの初期化
void* EnglishLayer::InitializeLayerdObject(void* obj, int classID)
{
	int layerID = _Private->_ID;
	if (classID == 0)
	{
		// キャスト
		EnglishLayer_Hello* layerdObject = reinterpret_cast<EnglishLayer_Hello*>(obj);
		// パーシャルクラスのためのメンバ変数
		layerdObject->_Private->_PartialClassMembers[layerID] = new EnglishLayer_Hello::PartialClassMembers();
		// レイヤ
		layerdObject->_Private->_PartialClassMembers[layerID]->_Layer = this;
		// パーシャルクラスの処分を行うメソッドへの関数ポインタ
		volatile void* vfp = DependentCode::GetLayerdObjectFinalizer(layerdObject);
		layerdObject->_Private->_PartialClassMembers[layerID]->_Finalizer = vfp;
		// ユーザ定義の初期化
		layerdObject->_RTCOP_InitializePartialClass();
	}

	return obj;
}

// アクティベート開始時に実行される
void EnglishLayer::_RTCOP_OnActivating()
{
	printf("%s Activating: %d\n", _Private->_Name, (int)_Private->_LayerState);
}

// アクティベート終了時に実行される
void EnglishLayer::_RTCOP_OnActivated()
{
	printf("%s Activated: %d\n", _Private->_Name, (int)_Private->_LayerState);
}

// ディアクティベート開始時に実行される
void EnglishLayer::_RTCOP_OnDeactivating()
{
	printf("%s Deactivating: %d\n", _Private->_Name, (int)_Private->_LayerState);
}

// ディアクティベート終了時に実行される
void EnglishLayer::_RTCOP_OnDeactivated()
{
	printf("%s Deactivated: %d\n", _Private->_Name, (int)_Private->_LayerState);
}

//--------------------------------------------------------------
// Helloの実装
//--------------------------------------------------------------
// コンストラクタ
// これは、仮想関数テーブル取得用
EnglishLayer_Hello::EnglishLayer_Hello(int a)
	: RTCOP::Core::LayerdObject<Hello>(a)
{
}

// パーシャルメソッド1
void EnglishLayer_Hello::Print()
{
	// 準備
	EnglishLayer_Hello::PartialClassMembers* layermembers = (EnglishLayer_Hello::PartialClassMembers*)_Private->_PartialClassMembers[2];
	volatile void* proceedaddr = Framework::Instance->GetRTCOPManager()->GetLayer(2)->GetVirtualFunctionTableForProceeding(0)[0];  // classId methodId(+offset)
	auto proceed = [this, proceedaddr]() { DependentCode::HelloClass::ExecuteProceed_Print(this, proceedaddr); };
	// レイヤ記述の内容
	printf("Print: Hello %d\n", layermembers->_EnglishMember);
	proceed();
}

// パーシャルクラスの初期化
void EnglishLayer_Hello::_RTCOP_InitializePartialClass()
{
	// レイヤ記述の内容
	EnglishLayer_Hello::PartialClassMembers* members = (EnglishLayer_Hello::PartialClassMembers*)_Private->_PartialClassMembers[2];
	members->_EnglishMember = 5;
}

// パーシャルクラスの終了処理
void EnglishLayer_Hello::_RTCOP_FinalizePartialClass()
{
	// レイヤ記述の内容
	EnglishLayer_Hello::PartialClassMembers* members = (EnglishLayer_Hello::PartialClassMembers*)_Private->_PartialClassMembers[2];
	members->_EnglishMember = 0;

	// 共通の終了処理
	/*今は無し*/
}

} // namespace Generated {}
} // namespace RTCOP {}
