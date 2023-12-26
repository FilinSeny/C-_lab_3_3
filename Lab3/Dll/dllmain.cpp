// dllmain.cpp : Определяет точку входа для приложения DLL.
#include "pch.h"
#include "mkl.h"

#include<iostream>
#include<vector>
#include<cerrno>
#include<string>
using namespace std;

const MKL_INT xhint = DF_UNIFORM_PARTITION;
const MKL_INT s_type = DF_PP_NATURAL;
const MKL_INT s_order = DF_PP_CUBIC; ///тут пока не уверен
const MKL_INT bc_type = DF_BC_FREE_END;


enum class ErrorEnum { NO, INIT, CHECK, SOLVE, JACOBI, GET, DEL, RCI };

///структура для передачи данных внутрь функции
struct DataInfoStruct
{
	double* SplineBorders;
	double* SplineCoefficients;
	int NNonUniformNodes;
	double* NonUniformNodes;
	double* NonUniformValues;
	bool IsFinal;
};


void deb_out(int l, double* arr) {
	for (int i = 0; i < l; ++i) {
		cerr << arr[i] << ' ';
	}
	cerr << endl;
}


constexpr int DIMENSION = 1;
constexpr int POW = 3;

void Spline2(MKL_INT *GridLen, MKL_INT *UniformGridLen, double * UniformGridVal, double *GridVal, void * data) {
	DataInfoStruct params = *((DataInfoStruct*)data);
	auto el = params.IsFinal;
	

	DFTaskPtr Task;
	MKL_INT status = dfdNewTask1D(&Task, *UniformGridLen, params.SplineBorders,
		DF_UNIFORM_PARTITION, DIMENSION, UniformGridVal, DF_NO_HINT);
	if (status != DF_STATUS_OK) throw "Error of creation spline with code " + std::to_string(status);

	double borderVal[2] = { 0, 0 };
	double* Coef = new double[DIMENSION * (POW + 1) * ((*UniformGridLen) -1)];
	status = dfdEditPPSpline1D(Task, DF_PP_CUBIC,
		DF_PP_NATURAL,
		DF_BC_2ND_LEFT_DER | DF_BC_2ND_RIGHT_DER,
		borderVal,
		DF_NO_IC,
		NULL,
		Coef,
		DF_NO_HINT);
	if (status != DF_STATUS_OK) throw "Error of creation spline with code " + std::to_string(status);

	status = dfdConstruct1D(Task, DF_PP_SPLINE, DF_METHOD_STD);
	if (status != DF_STATUS_OK) throw "Error of constuction spline with code " + std::to_string(status);

	

	int nDorder = 1;
	MKL_INT dorder[] = { 1 };
	status = dfdInterpolate1D(Task,
		DF_INTERP,
		DF_METHOD_PP,
		*GridLen,
		params.NonUniformNodes,
		DF_NON_UNIFORM_PARTITION,
		nDorder,
		dorder,
		NULL,
		GridVal,
		DF_NO_HINT,
		NULL);
	if (status != DF_STATUS_OK) throw "Error of interpolation spline with code " + std::to_string(status);

	
	status = dfDeleteTask(&Task);
	if (status != DF_STATUS_OK) throw "Error of deleting task with code " + std::to_string(status);
	
	if (!params.IsFinal) {
		
		for (int i = 0; i < *GridLen; ++i) {
			GridVal[i] = GridVal[i] - params.NonUniformValues[i];
		}
	}


	delete[] Coef;

}



extern "C" _declspec(dllexport)
void calculateSpline2(
	int GridLen, double* Grid, double* GridValues,
	double* NewGridValues,
	int LenUniformGrid,
	int MaxIterations,
	int& IterationsCount, double& Minresidual,
	int& ErrorCode) {

	
	MKL_INT MaxIterationNumber = MaxIterations;
	MKL_INT MaxStepIterations = 100; ///максимальное число итераций для выбора пробного шага
	MKL_INT NumberOfDoneIteration = 0;
	MKL_INT TerminationReason = 0;

	double eps0 = 30;///начальгле приближение

	const double eps[] = {///Массив ктритероме остановки
		1.0E-12,
		1.0E-12,
		1.0E-12,
		1.0E-12,
		1.0E-12,
		1.0E-12
	};

	MKL_INT DataInfo[6];

	
	double* UniformValues = new double[LenUniformGrid];
	
	double jacobianEps = 1.0E-8;
	
	double * JacobianMatrix = new double[ LenUniformGrid * GridLen];
	ErrorCode = 0;
	
	_TRNSP_HANDLE_t Task;
	MKL_INT status;
	DataInfoStruct Info;
	Info.IsFinal = false;
	double borders[2] = { Grid[0], Grid[LenUniformGrid - 1] };
	Info.SplineBorders = borders;
	Info.NonUniformValues = GridValues;
	Info.NonUniformNodes = Grid;
	int error = 0;

	///cout << "Ops4" << endl;

	

	try {
		status = dtrnlsp_init(&Task, &LenUniformGrid, &GridLen, UniformValues, eps, &MaxIterationNumber, &MaxStepIterations, &eps0);
		if (status != TR_SUCCESS) throw (ErrorEnum(ErrorEnum::INIT));

		

		status = dtrnlsp_check(&Task, &LenUniformGrid, &GridLen, JacobianMatrix, NewGridValues, eps, DataInfo);
		
		if (status != TR_SUCCESS) {
			cerr << status << endl;
			throw (ErrorEnum(ErrorEnum::CHECK));
		}
		

		MKL_INT RciRequest = 0;
		int i = 0;
		while (true)
		{
			++i;
			
			status = dtrnlsp_solve(&Task, NewGridValues, JacobianMatrix, &RciRequest);
			
			if (status != TR_SUCCESS) {
				cerr << "Status:" << status << endl << i << endl;
				throw (ErrorEnum(ErrorEnum::SOLVE));
			}

			if (RciRequest == 0) {
				///cerr << 1 << endl;
				continue;
			}
			else if (RciRequest == 1) {
				///cerr << 2 << endl;

				Spline2(&GridLen, &LenUniformGrid, UniformValues, NewGridValues, (void *) (&Info));
				
				continue;
			}
			else if (RciRequest == 2) {
				///cerr << 3 << endl;
				status = djacobix(Spline2, &LenUniformGrid, &GridLen, JacobianMatrix, UniformValues, &jacobianEps, (void*)(&Info));
				if (status != TR_SUCCESS) {
					cerr << status << endl;
					throw (ErrorEnum(ErrorEnum::JACOBI));
				}
				
				continue;
			}
			else if (RciRequest <= -1 && RciRequest >= -6) {
				///cout << RciRequest << endl;
				break;
			}
			else {
				throw(ErrorEnum(ErrorEnum::RCI));
			}
		}


		
		double InitialNorm = 0;
		double FinalNorm = 0;
		status = dtrnlsp_get(&Task, &NumberOfDoneIteration, &TerminationReason, &InitialNorm, &FinalNorm);
		if (status != TR_SUCCESS) {
			cerr << status << endl;
			throw (ErrorEnum(ErrorEnum::GET));
		}
		

		IterationsCount = NumberOfDoneIteration;
		ErrorCode = TerminationReason;
		Minresidual = FinalNorm;


		
		status = dtrnlsp_delete(&Task);
		if (status != TR_SUCCESS) {
			cerr << status << endl;
			throw (ErrorEnum(ErrorEnum::DEL));
		}

		
		Info.IsFinal = true;

		
		Spline2(&GridLen, &LenUniformGrid, UniformValues, NewGridValues, (void*)(&Info));
		
		
		delete[] JacobianMatrix;
	}
	catch (ErrorEnum _error) {
		error = (int) _error;
		cerr << "Error in iterations num of error: " << error << endl;

	}
	catch (const char* str)
	{
		cerr << string(str) << endl;
	}
}


extern "C" _declspec(dllexport)
void test(int LenGrid, double* Grid, double* GridValues, double *SplineValues,
	int Nspline) {
	DataInfoStruct di;
	di.IsFinal = false;
	 
	

	double borders[2] = { Grid[0], Grid[LenGrid - 1] };
	di.SplineBorders = borders;
	di.NonUniformNodes = Grid;
	di.NonUniformValues = GridValues;

	double* UniformValues = new double[Nspline];

	Spline2(&LenGrid, &Nspline, SplineValues, GridValues, (void*)(&di));

	cout << "results" << endl;
	///deb_out(Nspline, UniformValues);
	deb_out(LenGrid, GridValues);


}
