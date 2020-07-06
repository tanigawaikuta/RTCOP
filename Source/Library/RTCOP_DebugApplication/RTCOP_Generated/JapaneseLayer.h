//================================================================================
// JapaneseLayer.h
//
// 役割: JapaneseLayerを表すクラス。自動生成。
//================================================================================

#ifndef __RTCOP_GENERATED_JAPANESELAYER__
#define __RTCOP_GENERATED_JAPANESELAYER__

// 機能実現のためのヘッダ
#include "RTCOP/Core/Layer.h"
#include "RTCOP/Core/LayerdObject.h"
#include "RTCOP/Core/PartialClassMembers.h"
// JapaneseLayerに含まれるクラスのヘッダ
#include "Hello.h"

namespace RTCOP {
namespace Generated {

// Japaneseレイヤ
class JapaneseLayer : public Core::Layer
{
public:
	// インスタンスの取得
	static JapaneseLayer* GetInstance();
public:
	// コンストラクタ・デストラクタ
	JapaneseLayer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods);
	virtual ~JapaneseLayer();
protected:
	// レイヤードオブジェクトの初期化
	virtual void* InitializeLayerdObject(void* obj, int classID);
protected:
	// イベント発生時に実行されるメソッド
	virtual void OnActivating();		// アクティベート開始時に実行される
	virtual void OnActivated();			// アクティベート終了時に実行される
	virtual void OnDeactivating();		// ディアクティベート開始時に実行される
	virtual void OnDeactivated();		// ディアクティベート終了時に実行される
};

// JapaneseレイヤのHello
class JapaneseLayer_Hello : public Core::LayerdObject<Hello>
{
public:
	// パーシャルクラスのメンバ変数
	class PartialClassMembers : public RTCOP::Core::PartialClassMembers
	{
	public:
		int _JapaneseMember;
	};
public:
	JapaneseLayer_Hello();
	virtual void Print();
	virtual void Print2(char arg);
private:
	void _InitializePartialClass();
	void _FinalizePartialClass();
public:
	friend JapaneseLayer;
};

} // namespace Generated {}
} // namespace RTCOP {}

#endif //!__RTCOP_GENERATED_JAPANESELAYER__
