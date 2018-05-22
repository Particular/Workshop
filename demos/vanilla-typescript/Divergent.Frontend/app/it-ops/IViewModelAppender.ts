export interface IViewModelAppender {
    readonly requestIdentifier: string,
    append(viewModel: any, requestArgs: any): Promise<void>
}