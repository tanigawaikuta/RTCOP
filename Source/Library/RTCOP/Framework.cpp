//================================================================================
// Framework.cpp
//
// 役割: ユーザにRTCOPのフレームワークを提供する。RTCOPアプリケーションでは、
//       開始・終了時に、このヘッダで宣言されたメソッドを必ず実行しなければならない。
//================================================================================

#include "RTCOP/Framework.h"
#include "Framework_Private.h"
#include "RTCOP/Core/Initializer.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "RTCOP/Core/LayerdObjectInitializer.h"
#include "RTCOP/Core/LayerActivater.h"

namespace RTCOP {

//------------------------------------------------------
// プライベートクラスの実装
//------------------------------------------------------
// コンストラクタ
Framework_Private::Framework_Private()
	: _RTCOPManager(0), _LayerdObjectInitializer(0), _LayerActivater(0)
{
}

// デストラクタ
Framework_Private::~Framework_Private()
{
}


//------------------------------------------------------
// シングルトンクラスのインスタンス化
//------------------------------------------------------
Framework* const Framework::Instance = new Framework();

// 自動生成コード
namespace Generated {
// デフォルトイニシャライザの取得
extern Core::Initializer* _GetDefaultInitializer_();

} // namespace Generated

// ユーザが実行すべき、RTCOPの初期化
void Framework::Initialize()
{
	// デフォルトのイニシャライザの取得
	Core::Initializer* initializer = Generated::_GetDefaultInitializer_();
	// 初期化実行
	Initialize(initializer);
	// 後始末
	delete initializer;
}

// ユーザが実行すべき、RTCOPの初期化
void Framework::Initialize(Core::Initializer* initializer)
{
	// 各オブジェクトの生成
	_Private->_RTCOPManager = initializer->InitializeRTCOPManager();
	_Private->_LayerdObjectInitializer = initializer->InitializeLayerdObjectInitializer(_Private->_RTCOPManager);
	_Private->_LayerActivater = initializer->InitializeLayerActivater(_Private->_RTCOPManager);
	// レイヤの登録
	initializer->RegisterLayers(_Private->_RTCOPManager);
	// 初回レイヤアクティベーション
	initializer->FirstLayerActivation(_Private->_LayerActivater);

	// イニシャライザの後始末は、呼び出し側で行うこと…
}

// ユーザが実行すべき、RTCOPの終了処理のメソッド
void Framework::Finalize()
{
	// 各オブジェクトをdeleteする
	delete _Private->_LayerActivater;
	delete _Private->_LayerdObjectInitializer;
	delete _Private->_RTCOPManager;
	// 自殺
	delete this;
}

// RTCOPManagerの取得
Core::RTCOPManager* const Framework::GetRTCOPManager() const
{
	return _Private->_RTCOPManager;
}

// LayerdObjectInitializerの取得
Core::LayerdObjectInitializer* const Framework::GetLayerdObjectInitializer() const
{
	return _Private->_LayerdObjectInitializer;
}

// LayerActivaterの取得
Core::LayerActivater* const Framework::GetLayerActivater() const
{
	return _Private->_LayerActivater;
}

// コンストラクタ
Framework::Framework()
{
	_Private = new Framework_Private();
}

// デストラクタ
Framework::~Framework()
{
	delete _Private;
	_Private = 0;
}

} // namespace RTCOP {}
