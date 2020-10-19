//================================================================================
// JapaneseLayer.cpp
//
// 役割: JapaneseLayerを表すクラス。自動生成。
//================================================================================

#include "RTCOP_Generated/API.h"
#include "RTCOP_Generated/JapaneseLayer.h"
#include "RTCOP_Generated/Layer_Private.h"
#include "RTCOP_Generated/LayerdObject_Private.h"
#include "RTCOP/Framework.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "RTCOP_Generated/DependentCode/ExecuteProceed.h"
#include "RTCOP_Generated/DependentCode/GetVirtualFunctionTable.h"
#include "RTCOP_Generated/DependentCode/GetLayerdObjectFinalizer.h"
//#include <iostream>
//#include <cstring>

#include "Hello.h"
#include <stdio.h>

namespace RTCOP {
namespace Generated {

//--------------------------------------------------------------
// レイヤの実装
//--------------------------------------------------------------
// インスタンスの取得
JapaneseLayer* JapaneseLayer::GetInstance()
{
	return (JapaneseLayer*)RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer((int)LayerID::JapaneseLayer);
}

// コンストラクタ
JapaneseLayer::JapaneseLayer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)
	:RTCOP::Core::Layer(id, name, numOfBaseClasses, numOfBaseMethods)
{
	// Helloクラスの仮想関数テーブル
	//int size0 = sizeof(volatile void*) * numOfBaseMethods[0];
	volatile void** virtualFunctionTable0 = DependentCode::HelloClass::GetVirtualFunctionTable(this);
	for (int i = 0; i < numOfBaseMethods[0]; ++i)
	{
		_Private->_VirtualFunctionTables[0][i] = virtualFunctionTable0[i];
	}
	//std::memcpy(_Private->_VirtualFunctionTables[0], virtualFunctionTable0, size0);
	// 再定義なしのメソッドやデストラクタは0にしておく
	_Private->_VirtualFunctionTables[0][2] = 0;
	_Private->_VirtualFunctionTables[0][3] = 0;
#if defined(LINUX_X86) || defined(LINUX_X64) || defined(LINUX_ARM) || defined(LINUX_ARM64) || defined(WIN32_MINGW) || defined(WIN64_MINGW) || defined(MACOS_X64)
	_Private->_VirtualFunctionTables[0][4] = 0;
#endif
}

// デストラクタ
JapaneseLayer::~JapaneseLayer()
{
}

// レイヤードオブジェクトの初期化
void* JapaneseLayer::InitializeLayerdObject(void* obj, int classID)
{
	int layerID = _Private->_ID;
	if(classID == 0)
	{
		// キャスト
		JapaneseLayer_Hello* layerdObject = reinterpret_cast<JapaneseLayer_Hello*>(obj);
		// パーシャルクラスのためのメンバ変数
		layerdObject->_Private->_PartialClassMembers[layerID] = new JapaneseLayer_Hello::PartialClassMembers();
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
void JapaneseLayer::_RTCOP_OnActivating()
{
	printf("%s Activating: %d\n", _Private->_Name, (int)_Private->_LayerState);
}

// アクティベート終了時に実行される
void JapaneseLayer::_RTCOP_OnActivated()
{
	printf("%s Activated: %d\n", _Private->_Name, (int)_Private->_LayerState);
}

// ディアクティベート開始時に実行される
void JapaneseLayer::_RTCOP_OnDeactivating()
{
	printf("%s Deactivating: %d\n", _Private->_Name, (int)_Private->_LayerState);
}

// ディアクティベート終了時に実行される
void JapaneseLayer::_RTCOP_OnDeactivated()
{
	printf("%s Deactivated: %d\n", _Private->_Name, (int)_Private->_LayerState);
}

//--------------------------------------------------------------
// Helloの実装
//--------------------------------------------------------------
// コンストラクタ
// これは、仮想関数テーブル取得用
JapaneseLayer_Hello::JapaneseLayer_Hello(int a)
	: RTCOP::Core::LayerdObject<Hello>(a)
{
}

// パーシャルメソッド1
void JapaneseLayer_Hello::Print()
{
	// 準備
	JapaneseLayer_Hello::PartialClassMembers* members = (JapaneseLayer_Hello::PartialClassMembers*)_Private->_PartialClassMembers[1];
	volatile void* proceedaddr = Framework::Instance->GetRTCOPManager()->GetLayer(1)->GetVirtualFunctionTableForProceeding(0)[0];  // classId methodId(+offset)
	auto proceed = [this, proceedaddr]() { DependentCode::HelloClass::ExecuteProceed_Print(this, proceedaddr); };
	// レイヤ記述の内容
	printf("Print: こんにちは\n");
	proceed();
}

// パーシャルメソッド2
void JapaneseLayer_Hello::Print2(char arg)
{
	// 準備
	JapaneseLayer_Hello::PartialClassMembers* members = (JapaneseLayer_Hello::PartialClassMembers*)_Private->_PartialClassMembers[1];
	volatile void* proceedaddr = Framework::Instance->GetRTCOPManager()->GetLayer(1)->GetVirtualFunctionTableForProceeding(0)[0];  // classId methodId(+offset)
	auto proceed = [this, proceedaddr](char arg) { DependentCode::HelloClass::ExecuteProceed_Print2(this, proceedaddr, arg); };
	// レイヤ記述の内容
	printf("Print2: こんにちは %d\n", members->_JapaneseMember);
	proceed(arg);
}

// パーシャルクラスの初期化
void JapaneseLayer_Hello::_RTCOP_InitializePartialClass()
{
	// レイヤ記述の内容
	JapaneseLayer_Hello::PartialClassMembers* members = (JapaneseLayer_Hello::PartialClassMembers*)_Private->_PartialClassMembers[1];
	members->_JapaneseMember = 3;
}

// パーシャルクラスの終了処理
void JapaneseLayer_Hello::_RTCOP_FinalizePartialClass()
{
	// レイヤ記述の内容
	JapaneseLayer_Hello::PartialClassMembers* members = (JapaneseLayer_Hello::PartialClassMembers*)_Private->_PartialClassMembers[1];
	members->_JapaneseMember = 0;
}

} // namespace Generated {}
} // namespace RTCOP {}
